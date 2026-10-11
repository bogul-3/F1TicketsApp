using F1Tickets.Api.Domain.Lookups;
using F1Tickets.Api.Events.PurchaseTicket;
using F1Tickets.Api.Features.Currencies.Interfaces;
using F1Tickets.Api.Features.Tickets.PurchaseTicket.DTOs;
using F1Tickets.Api.Features.Tickets.PurchaseTicket.Interfaces;
using F1Tickets.Api.Infrastructure.Cache;
using F1Tickets.Api.Infrastructure.Messaging;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

public class PurchaseService : IPurchaseService
{
    private readonly F1TicketsDbContext _db;
    private readonly IRabbitMqApiPublisher _publisher;
    private readonly RedisCacheService _cache;
    private readonly ICurrencyConversionService _currencyConversionService;

    public PurchaseService(
        F1TicketsDbContext db,
        IRabbitMqApiPublisher publisher,
        RedisCacheService cache,
        ICurrencyConversionService currencyConversionService)
    {
        _db = db;
        _publisher = publisher;
        _cache = cache;
        _currencyConversionService = currencyConversionService;
    }

    public async Task<PurchaseResponseDto> PurchaseTicketsAsync(PurchaseRequestDto dto)
    {
        Console.WriteLine($"Purchase request received: userId={dto.userId}, zoneDayId={dto.ZoneDayId}, quantity={dto.Quantity}");
        // 1 Load ZoneDay
        var zoneDay = await _db.ZoneDays
            .Include(z => z.Zone)
            .Include(z => z.RaceDay)
                .ThenInclude(r => r.RaceEvent)
            .FirstOrDefaultAsync(z => z.Id == dto.ZoneDayId);
        if (zoneDay == null)
            throw new Exception("ZoneDay not found");

        // 2 Load user
        var user = await _db.Users.FindAsync(dto.userId);
        if (user == null)
        {
            throw new Exception($"[API] {dto.userId}user not found");
        }

        // 3 Load PromoCode if provided
        PromoCode? promo = null;
        if (!string.IsNullOrWhiteSpace(dto.PromoCode))
        {
            promo = await _db.PromoCodes
                .FirstOrDefaultAsync(p => p.Code == dto.PromoCode);

            if (promo == null || promo.IsUsed)
                throw new Exception("Invalid promo code");
        }

        // 4 Calculate single ticket price
        decimal singlePrice = zoneDay.PriceAddon + zoneDay.RaceDay.BasePrice;

        // 5 10% automatic discount if within valid period
        if (DateTime.UtcNow <= zoneDay.RaceDay.RaceEvent.DiscountValidUntil)
            singlePrice *= 0.9m;

        // 6 Apply promo code discount if any
        if (promo != null)
            singlePrice *= (1 - promo.DiscountPercentage / 100);

        // 7 Convert to requested currency if different
        string currencyCode = string.IsNullOrEmpty(dto.CurrencyCode) ? "EUR" : dto.CurrencyCode;
        if (currencyCode != "EUR")
        {
            singlePrice = await _currencyConversionService.ConvertAsync(singlePrice, "EUR", currencyCode);
        }

        // 8 Generate new promo code for next purchase
        var newPromo = new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = GeneratePromoCode(),
            DiscountPercentage = 5,
            IsUsed = false
        };
        _db.PromoCodes.Add(newPromo);
        await _db.SaveChangesAsync();

        // 9 Create Purchase Request Event
        var purchaseId = Guid.NewGuid();
        var purchaseEvent = new TicketPurchaseRequestedEvent
        {
            PurchaseRequestId = purchaseId,
            userId = user.Id,
            CurrencyCode = currencyCode,
            ZoneDayId = zoneDay.Id,
            Quantity = dto.Quantity,
            PromoCodeId = promo?.Id,
            TotalPrice = singlePrice,
        };

        // 10 Publish Purchase Request Event
        await _publisher.PublishPurchaseAsync(purchaseEvent);
        Console.WriteLine($"Published event for user {purchaseEvent.userId}");

        // 11 Redis invalidate
        if (_cache != null)
        {
            var raceEventId = zoneDay.RaceDay.RaceEventId;
            await _cache.KeyDeleteAsync($"raceeventfull:{raceEventId}");
            Console.WriteLine($"Redis cache invalidated for RaceEvent {raceEventId}");
        }

        // 12 Return Response DTO
        return new PurchaseResponseDto
        {
            PurchaseRequestId = purchaseId,
            TotalPrice = singlePrice,
            PromoCode = newPromo.Code,
            PromoCodeId = newPromo.Id,
            CurrencyCode = currencyCode
        };
    }

    private string GeneratePromoCode()
    {
        return Guid.NewGuid().ToString("N")[..8].ToUpper();
    }
}