using F1Tickets.Api.Domain.Common;

namespace F1Tickets.Api.Domain.Lookups
{
    public class City : BaseEntity
    {
        public string Name { get; set; }

        // RELATIONSHIPS
        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }
}
