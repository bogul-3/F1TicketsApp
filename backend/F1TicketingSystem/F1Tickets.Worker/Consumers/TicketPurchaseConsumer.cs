using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Domain.Lookups;
using F1Tickets.Api.Events.PurchaseTicket;
using F1Tickets.Api.Features.Currencies.Interfaces;
using F1Tickets.Api.Infrastructure.Cache;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Worker.Consumers
{
    public class TicketPurchaseConsumer
    {
        private readonly F1TicketsDbContext _db;
        private readonly IRabbitMqWorkerPublisher _publisher;
        private readonly RedisCacheService _cache;

        public TicketPurchaseConsumer(
            F1TicketsDbContext db,
            IRabbitMqWorkerPublisher publisher,
            RedisCacheService cache)
        {
            _db = db;
            _publisher = publisher;
            _cache = cache;
        }

        public async Task HandleAsync(TicketPurchaseRequestedEvent evt)
        {
            Console.WriteLine($"[Worker] Processing purchase request {evt.PurchaseRequestId}");

            // 1 Acquire Redis lock
            var lockKey = $"lock:zoneday:{evt.ZoneDayId}";
            var lockValue = Guid.NewGuid().ToString();
            var acquired = await _cache.AcquireLockAsync(lockKey, lockValue, TimeSpan.FromSeconds(10));
            if (!acquired)
            {
                Console.WriteLine("[Worker] Could not acquire lock for ZoneDay");
                return;
            }

            try
            {
                // 2 Idempotency check
                if (await _db.Tickets.AnyAsync(t => t.PurchaseRequestId == evt.PurchaseRequestId))
                {
                    Console.WriteLine($"[Worker] Duplicate PurchaseRequest detected: {evt.PurchaseRequestId}");
                    return;
                }

                // 3 Load ZoneDay
                var zoneDay = await _db.ZoneDays
                    .Include(z => z.Zone)
                    .Include(z => z.RaceDay)
                        .ThenInclude(rd => rd.RaceEvent)
                    .FirstOrDefaultAsync(z => z.Id == evt.ZoneDayId);
                if (zoneDay == null)
                {
                    Console.WriteLine($"[Worker] ZoneDay not found: {evt.ZoneDayId}");
                    return;
                }

                // 4 Load user
                var user = await _db.Users.FindAsync(evt.userId);
                if (user == null)
                {
                    Console.WriteLine($"[Worker] user not found: {evt.userId}");
                    return;
                }

                // 5 Load PromoCode
                PromoCode? promo = null;
                if (evt.PromoCodeId.HasValue)
                {
                    promo = await _db.PromoCodes.FindAsync(evt.PromoCodeId.Value);
                    if (promo == null || promo.IsUsed)
                    {
                        Console.WriteLine($"[Worker] Invalid promo code: {evt.PromoCodeId}");
                        promo = null;
                    }
                }

                // 6 Atomic seat reservation
                var rowsAffected = await _db.Database.ExecuteSqlInterpolatedAsync($@"
                    UPDATE zone_days
                    SET seats_left = seats_left - {evt.Quantity}
                    WHERE id = {evt.ZoneDayId} AND seats_left >= {evt.Quantity}");

                if (rowsAffected == 0)
                {
                    Console.WriteLine($"[Worker] Not enough seats left for ZoneDay {evt.ZoneDayId}");
                    return;
                }

                // 7 Create tickets
                var tickets = new List<Ticket>();
                string currencyCode = string.IsNullOrEmpty(evt.CurrencyCode) ? "EUR" : evt.CurrencyCode;

                for (int i = 0; i < evt.Quantity; i++)
                {
                    tickets.Add(new Ticket
                    {
                        Id = Guid.NewGuid(),
                        PurchaseRequestId = evt.PurchaseRequestId,
                        UserId = user.Id,
                        ZoneDayId = zoneDay.Id,
                        CurrencyCode = currencyCode,
                        PromoCodeId = promo?.Id,
                        TotalPrice = evt.TotalPrice,
                        Status = TicketStatus.ACTIVE,
                        PurchasedOn = DateTime.UtcNow,
                        AccessCode = Guid.NewGuid().ToString()[..8].ToUpper()
                    });
                }

                await using var transaction = await _db.Database.BeginTransactionAsync();

                try
                {
                    // 8 Insert into db
                    await _db.Tickets.AddRangeAsync(tickets);
                    if (promo != null)
                        promo.IsUsed = true;
                    await _db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    // 9 Invalidate cache
                    await _cache.KeyDeleteAsync($"zoneday:{zoneDay.Id}");
                    await _cache.KeyDeleteAsync($"raceeventfull:{zoneDay.RaceDay.RaceEventId}");
                }
                catch(DbUpdateConcurrencyException)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("[Worker] Concurrency conflict - seat already taken.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"[Worker] Transaction failed: {ex.Message}");
                    return;
                }

                // 10 Publish Purchase Event for Reporting
                foreach (var ticket in tickets)
                {
                    var purchasedEvent = new TicketPurchasedEvent
                    {
                        TicketId = ticket.Id,
                        userId = ticket.UserId,
                        ZoneDayId = ticket.ZoneDayId,
                        RaceDayId = zoneDay.RaceDayId,
                        RaceEventId = zoneDay.RaceDay.RaceEventId,
                        TotalPrice = ticket.TotalPrice,
                        Quantity = 1,
                        PurchasedOn = ticket.PurchasedOn,
                        CurrencyCode = currencyCode
                    };

                    await _publisher.PublishAsync(purchasedEvent);
                    Console.WriteLine($"[Worker] Published TicketPurchasedEvent for Ticket {ticket.Id}");
                }

                Console.WriteLine($"[Worker] Purchase completed for request {evt.PurchaseRequestId}");
            }
            finally
            {
                await _cache.ReleaseLockAsync(lockKey);
            }
        }
    }
}