using F1Tickets.Api.Events.PurchaseTicket;
using F1Tickets.Reporting.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Reporting.Api.Consumers
{
    public class ReportingTicketPurchasedConsumer
    {
        private readonly F1ReportingDbContext _db;
        public ReportingTicketPurchasedConsumer(F1ReportingDbContext db)
        {
            _db = db;
        }

        public async Task HandleAsync(TicketPurchasedEvent evt)
        {
            try
            {
                // 1 Check if Ticket exists
                var alreadyExists = await _db.ReportingTickets
                    .AnyAsync(t => t.TicketId == evt.TicketId);
                if (alreadyExists)
                {
                    Console.WriteLine($"[Reporting] Ticket {evt.TicketId} already exists in reporting DB");
                    return;
                }

                // 2 Create Ticket
                var reportingTicket = new ReportingTicket
                {
                    Id = evt.TicketId,
                    TicketId = evt.TicketId,
                    userId = evt.userId,
                    ZoneDayId = evt.ZoneDayId,
                    RaceDayId = evt.RaceDayId,
                    RaceEventId = evt.RaceEventId,
                    Quantity = evt.Quantity,
                    TotalPrice = evt.TotalPrice,
                    PurchasedOn = evt.PurchasedOn
                };

                // 3 Insert Ticket into db
                await _db.ReportingTickets.AddAsync(reportingTicket);
                await _db.SaveChangesAsync();

                Console.WriteLine($"[Reporting] Added ticket {evt.TicketId} for RaceDay {evt.RaceDayId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Reporting] Error processing event: {ex.Message}");
            }
        }
    }
}
