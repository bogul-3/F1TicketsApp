namespace F1Tickets.Api.Features.Locations.DTOs
{
    public class CityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CountryId { get; set; }
    }
}
