using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Events.UpdateTicket;
using F1Tickets.Api.Features.Zones.DTOs;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace F1Tickets.Worker.Consumers
{
    public class TicketUpdateConsumer
    {
        private readonly F1TicketsDbContext _db;
        private readonly IRabbitMqWorkerPublisher _publisher;

        public TicketUpdateConsumer(F1TicketsDbContext db, IRabbitMqWorkerPublisher publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task HandleAsync(TicketUpdateRequestedEvent evt)
        {
            Console.WriteLine($"[Worker] Update request for ticket {evt.TicketId}");

            Console.WriteLine($"TicketId={evt.TicketId}, AccessCode={evt.AccessCode}, Email={evt.Email}, RemoveZoneDayId={evt.RemoveZoneDayId} , AddZoneDayId{evt.AddZoneDayId}");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1️ Load original Ticket
                var ticket = await _db.Tickets
                    .Include(t => t.User)
                    .Include(t => t.PromoCode)
                    .Include(t => t.ZoneDay)
                        .ThenInclude(z => z.RaceDay)
                    .FirstOrDefaultAsync(t =>
                        t.Id == evt.TicketId &&
                        t.AccessCode == evt.AccessCode &&
                        t.User.Email == evt.Email &&
                        t.Status == TicketStatus.ACTIVE);
                if (ticket == null)
                {
                    Console.WriteLine("[Worker] Ticket not found or inactive.");
                    await transaction.RollbackAsync();
                    return;
                }

                // 2 Validate old ZoneDay
                if (ticket.ZoneDayId != evt.RemoveZoneDayId)
                {
                    Console.WriteLine("[Worker] RemoveZoneDay mismatch.");
                    await transaction.RollbackAsync();
                    return;
                }

                // 3 Load new ZoneDay
                var addZoneDay = await _db.ZoneDays
                    .Include(z => z.RaceDay)
                        .ThenInclude(r => r.RaceEvent)
                    .FirstOrDefaultAsync(z => z.Id == evt.AddZoneDayId);
                if (addZoneDay == null || addZoneDay.SeatsLeft <= 0)
                {
                    Console.WriteLine("[Worker] AddZoneDay not available.");
                    await transaction.RollbackAsync();
                    return;
                }

                // 4 Cancel old Ticket and Free seat
                ticket.Status = TicketStatus.CANCELLED;
                ticket.ZoneDay.SeatsLeft += 1;

                // 5 Calculate Price
                decimal totalPrice = addZoneDay.RaceDay.BasePrice + addZoneDay.PriceAddon;

                // 6 Calculate Discount
                if (addZoneDay.RaceDay.RaceEvent.DiscountValidUntil > DateTime.UtcNow)
                {
                    totalPrice *= 0.9m;
                }

                // 7 Check PromoCode
                if (ticket.PromoCode != null)
                    totalPrice *= (1 - ticket.PromoCode.DiscountPercentage / 100);

                // 8 Reserve new seat
                addZoneDay.SeatsLeft -= 1;

                // 9 Create new Ticket
                var newTicket = new Ticket
                {
                    Id = Guid.NewGuid(),
                    UserId = ticket.UserId,
                    ZoneDayId = addZoneDay.Id,
                    PromoCodeId = ticket.PromoCodeId,
                    TotalPrice = totalPrice,
                    PurchasedOn = DateTime.UtcNow,
                    Status = TicketStatus.ACTIVE,
                    AccessCode = Guid.NewGuid().ToString()[..8].ToUpper(),
                    PurchaseRequestId = Guid.NewGuid()
                };

                _db.Tickets.Update(ticket);
                _db.ZoneDays.Update(ticket.ZoneDay);
                _db.ZoneDays.Update(addZoneDay);
                _db.Tickets.Add(newTicket);

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // 10 Publish Update Event for Reporting
                var updateEvent = new TicketUpdatedEvent
                {
                    OldTicketId = ticket.Id,
                    OldTotalPrice = ticket.TotalPrice,
                    OldPurchasedOn = ticket.PurchasedOn,

                    TicketId = newTicket.Id,
                    userId = newTicket.UserId,
                    Status = newTicket.Status,
                    TotalPrice = newTicket.TotalPrice,
                    PurchasedOn = newTicket.PurchasedOn,
                    AccessCode = newTicket.AccessCode,
                    UpdatedZoneDay = new ZoneDayInfo
                    {
                        ZoneDayId = newTicket.ZoneDayId,
                        RaceDayId = addZoneDay.RaceDayId,
                        RaceEventId = addZoneDay.RaceDay.RaceEventId
                    }
                };

                await _publisher.PublishAsync(updateEvent);

                Console.WriteLine("[Worker] Ticket update processed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[Worker] ERROR: {ex.Message}");
            }
        }
    }
}