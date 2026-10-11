namespace F1Tickets.Api.Features.Tickets.PurchaseTicket.DTOs
{
    public class PurchaseResponseDto
    {
        public Guid PurchaseRequestId { get; set; }
        public List<Guid> TicketIds { get; set; } 
        public decimal TotalPrice { get; set; }
        public string PromoCode { get; internal set; } 
        public Guid PromoCodeId { get; internal set; }
        public string CurrencyCode { get; set; } 
    }
}
