using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Events.UpdateTicket;
using F1Tickets.Reporting.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Reporting.Api.Consumers
{
    public class ReportingTicketUpdatedConsumer
    {
        private readonly F1ReportingDbContext _db;

        public ReportingTicketUpdatedConsumer(F1ReportingDbContext db)
        {
            _db = db;
        }

        public async Task HandleAsync(TicketUpdatedEvent evt)
        {
            Console.WriteLine($"[Reporting] Received TicketUpdatedEvent: Ticket={evt.TicketId}");

            await using var transaction = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1️ Change old Ticket Status to Cancelled
                var oldTicket = await _db.ReportingTickets
                    .FirstOrDefaultAsync(t => t.TicketId == evt.OldTicketId);
                if (oldTicket != null)
                {
                    oldTicket.Status = TicketStatus.CANCELLED;
                    _db.ReportingTickets.Update(oldTicket);
                    Console.WriteLine($"[Reporting] Cancelled old ticket {evt.OldTicketId}");
                }

                // 2️ Create new Ticket
                var newTicket = new ReportingTicket
                {
                    Id = Guid.NewGuid(),
                    TicketId = evt.TicketId,
                    userId = evt.userId,
                    ZoneDayId = evt.UpdatedZoneDay.ZoneDayId,
                    RaceDayId = evt.UpdatedZoneDay.RaceDayId,
                    RaceEventId = evt.UpdatedZoneDay.RaceEventId,
                    Quantity = 1,
                    TotalPrice = evt.TotalPrice,
                    PurchasedOn = evt.PurchasedOn,
                    Status = evt.Status
                };

                // 3 Insert new Ticket into db
                await _db.ReportingTickets.AddAsync(newTicket);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                Console.WriteLine($"[Reporting] Added new ticket {evt.TicketId}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"[Reporting] ERROR: {ex.Message}");
            }
        }
    }
}