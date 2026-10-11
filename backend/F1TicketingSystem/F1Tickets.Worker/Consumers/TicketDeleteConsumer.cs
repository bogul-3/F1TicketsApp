using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Events.DeleteTicket;
using F1Tickets.Api.Events.UpdateTicket;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Worker.Consumers
{
    public class TicketDeleteConsumer
    {
        private readonly F1TicketsDbContext _db;
        private readonly IRabbitMqWorkerPublisher _publisher;

        public TicketDeleteConsumer(
            F1TicketsDbContext db,
            IRabbitMqWorkerPublisher publisher)
        {
            _db = db;
            _publisher = publisher;
        }

        public async Task HandleAsync(TicketDeleteRequestedEvent evt)
        {
            Console.WriteLine($"[Worker] Delete request for ticket {evt.TicketId}");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1 Get Ticket from db
                var ticket = await _db.Tickets
                    .Include(t => t.PromoCode)
                    .Include(t => t.User)
                    .Include(t => t.ZoneDay)
                        .ThenInclude(z => z.RaceDay)
                    .FirstOrDefaultAsync(t =>
                        t.Id == evt.TicketId &&
                        t.AccessCode == evt.AccessCode &&
                        t.User.Email == evt.Email &&
                        t.Status == TicketStatus.ACTIVE);
                if (ticket == null)
                {
                    Console.WriteLine("[Worker] Ticket not found.");
                    await transaction.RollbackAsync();
                    return;
                }

                // 2 Set Ticket Status to Cancel
                ticket.Status = TicketStatus.CANCELLED;

                // 3 Free Seat
                ticket.ZoneDay.SeatsLeft += 1;

                // 4 Free PromoCode
                if (ticket.PromoCode != null)
                {
                    ticket.PromoCode.IsUsed = false;
                }

                // 5 Update db
                _db.Tickets.Update(ticket);
                _db.ZoneDays.Update(ticket.ZoneDay);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                // 5 Publish Delete Event for Reporting
                await _publisher.PublishAsync(new TicketDeletedEvent
                {
                    TicketId = ticket.Id,
                    userId = ticket.UserId,
                    Status = TicketStatus.CANCELLED,
                    TotalPrice = ticket.TotalPrice,
                    PurchasedOn = ticket.PurchasedOn,
                    ZoneDay = new ZoneDayInfo
                    {
                        ZoneDayId = ticket.ZoneDayId,
                        RaceDayId = ticket.ZoneDay.RaceDayId,
                        RaceEventId = ticket.ZoneDay.RaceDay.RaceEventId
                    }
                });

                Console.WriteLine("[Worker] Ticket deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[Worker] ERROR: {ex.Message}");
            }
        }
    }
}