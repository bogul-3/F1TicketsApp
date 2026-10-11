using F1Tickets.Api.Domain.Common;

namespace F1Tickets.Api.Domain.Entities
{
    public class Zone : BaseEntity
    {
        public string Name { get; set; } 

        // RELATIONSHIPS
        public List<ZoneDay> ZoneDays { get; set; } 
    }
}
