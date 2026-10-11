using F1Tickets.Api.Domain.Common;

namespace F1Tickets.Api.Domain.Lookups
{
    public class PromoCode : BaseEntity
    {
        public string Code { get; set; } 
        public decimal DiscountPercentage { get; set; }
        public bool IsUsed { get; set; }
    }
}
