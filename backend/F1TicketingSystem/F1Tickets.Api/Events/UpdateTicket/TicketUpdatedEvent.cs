using F1Tickets.Api.Domain.Enums;

namespace F1Tickets.Api.Events.UpdateTicket
{
    public class TicketUpdatedEvent
    {
        // Stari tiket
        public Guid OldTicketId { get; set; }
        public decimal OldTotalPrice { get; set; }
        public DateTime OldPurchasedOn { get; set; }

        // Novi tiket
        public Guid TicketId { get; set; }
        public Guid userId { get; set; }
        public TicketStatus Status { get; set; } 
        public decimal TotalPrice { get; set; }
        public DateTime PurchasedOn { get; set; }

        public ZoneDayInfo UpdatedZoneDay { get; set; } 
        public string AccessCode { get; set; } 
    }

    public class ZoneDayInfo
    {
        public Guid ZoneDayId { get; set; }
        public Guid RaceDayId { get; set; }
        public Guid RaceEventId { get; set; }
    }
}
