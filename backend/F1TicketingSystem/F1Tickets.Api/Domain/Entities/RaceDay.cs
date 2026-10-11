using F1Tickets.Api.Domain.Common;

namespace F1Tickets.Api.Domain.Entities
{
    public class RaceDay : BaseEntity
    {
        public DateTime Date { get; set; }
        public decimal BasePrice { get; set; }

        // RELATIONSHIPS
        public Guid RaceEventId { get; set; }
        public RaceEvent RaceEvent { get; set; }
        public List<ZoneDay> ZoneDays { get; set; }
    }
}