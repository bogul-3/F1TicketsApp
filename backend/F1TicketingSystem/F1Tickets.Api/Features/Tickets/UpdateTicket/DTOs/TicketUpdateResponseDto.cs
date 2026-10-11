using F1Tickets.Api.Domain.Enums;

namespace F1Tickets.Api.Features.Tickets.UpdateTicket.DTOs
{
    public class TicketUpdateResponseDto
    {
        public Guid TicketId { get; set; }
        public decimal TotalPrice { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime PurchasedOn { get; set; }
        public string AccessCode { get; set; } 
        public Guid userId { get; set; }
        public TicketUpdateZoneDayDto UpdatedZoneDay { get; set; } 
    }
}
