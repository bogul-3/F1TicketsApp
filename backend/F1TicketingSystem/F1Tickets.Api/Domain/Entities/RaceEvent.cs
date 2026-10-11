using F1Tickets.Api.Domain.Common;
using F1Tickets.Api.Domain.Lookups;

namespace F1Tickets.Api.Domain.Entities
{
    public class RaceEvent : BaseEntity
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AdditionalInfo { get; set; }  
        public DateTime DiscountValidUntil { get; set; }

        // RELATIONSHIPS
        public List<RaceDay> RaceDays { get; set; }
        public Guid CityId { get; set; }
        public City City { get; set; } 
    }
}
