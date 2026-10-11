using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Events.DeleteTicket;
using F1Tickets.Reporting.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Reporting.Api.Consumers
{
    public class ReportingTicketDeletedConsumer
    {
        private readonly F1ReportingDbContext _db;

        public ReportingTicketDeletedConsumer(F1ReportingDbContext db)
        {
            _db = db;
        }

        public async Task HandleAsync(TicketDeletedEvent evt)
        {
            // 1 Find Ticket by Id
            var ticket = await _db.ReportingTickets
                .FirstOrDefaultAsync(t => t.TicketId == evt.TicketId);
            if (ticket == null)
            {
                Console.WriteLine($"[Reporting] Ticket {evt.TicketId} not found.");
                return;
            }

            // 2 Change Status to Cancelled
            ticket.Status = TicketStatus.CANCELLED;

            // 3 Insert Change into db
            await _db.SaveChangesAsync();

            Console.WriteLine($"[Reporting] Ticket {evt.TicketId} cancelled.");
        }
    }
}
