namespace F1Tickets.Api.Events.PurchaseTicket
{
    public class TicketPurchaseRequestedEvent
    {
        public Guid PurchaseRequestId { get; set; }
        public Guid userId { get; set; }
        public Guid ZoneDayId { get; set; }
        public int Quantity { get; set; }
        public Guid? PromoCodeId { get; set; }
        public string CurrencyCode { get; set; } 
        public decimal TotalPrice { get; set; }
    }
}
