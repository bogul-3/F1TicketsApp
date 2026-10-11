namespace F1Tickets.Api.Features.Zones.DTOs
{
    public class ZoneDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public List<ZoneDayDto> ZoneDays { get; set; } 
    }

    public class ZoneDayDto
    {
        public Guid Id { get; set; }
        public int Capacity { get; set; }
        public int SeatsLeft { get; set; }
        public decimal PriceAddon { get; set; }
    }
}
