namespace F1Tickets.Api.Features.Tickets.PurchaseTicket.DTOs
{
    public class PurchaseRequestDto
    {
        public Guid userId { get; set; }
        public Guid ZoneDayId { get; set; }
        public string PromoCode { get; set; }
        public int Quantity { get; set; }
        public string CurrencyCode { get; set; } 
    }
}
