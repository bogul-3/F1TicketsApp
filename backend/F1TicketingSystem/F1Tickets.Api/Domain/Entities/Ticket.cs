using F1Tickets.Api.Domain.Common;
using F1Tickets.Api.Domain.Enums;
using F1Tickets.Api.Domain.Lookups;

namespace F1Tickets.Api.Domain.Entities
{
    public class Ticket : BaseEntity
    {
        public decimal TotalPrice { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime PurchasedOn { get; set; } 
        public string AccessCode { get; set; } 
        public string CurrencyCode { get; set; } 
        public Guid PurchaseRequestId { get; set; }

        // RELATIONSHIPS
        public Guid UserId { get; set; }
        public User User { get; set; } 

        public Guid? PromoCodeId { get; set; }
        public PromoCode PromoCode { get; set; } 

        public Guid ZoneDayId { get; set; } 
        public ZoneDay ZoneDay { get; set; } 
    }
}
