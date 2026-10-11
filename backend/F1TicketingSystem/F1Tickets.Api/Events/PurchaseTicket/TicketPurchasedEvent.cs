namespace F1Tickets.Api.Events.PurchaseTicket
{
    public class TicketPurchasedEvent
    {
        public Guid TicketId { get; set; }
        public Guid userId { get; set; }
        public Guid ZoneDayId { get; set; }
        public Guid RaceDayId { get; set; }
        public Guid RaceEventId { get; set; }

        public int Quantity { get; set; }

        public DateTime PurchasedOn { get; set; }
        public decimal TotalPrice { get; set; }
        public string CurrencyCode { get; set; } 
    }
}
