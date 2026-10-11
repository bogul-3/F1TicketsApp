using F1Tickets.Api.Domain.Enums;
using global::F1Tickets.Api.Events.UpdateTicket;

namespace F1Tickets.Api.Events.DeleteTicket
{
    public class TicketDeletedEvent
    {
        public Guid TicketId { get; set; }

        public Guid userId { get; set; }

        public TicketStatus Status { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime PurchasedOn { get; set; }

        public ZoneDayInfo ZoneDay { get; set; } 
    }
}
