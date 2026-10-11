namespace F1Tickets.Api.Features.ZoneDays.DTOs
{
    public class ZoneDayDto
    {
        public Guid Id { get; set; }
        public Guid ZoneId { get; set; }

        public string ZoneName { get; set; } 
        public Guid RaceDayId { get; set; }
        public int Capacity { get; set; }
        public int SeatsLeft { get; set; }
        public decimal PriceAddon { get; set; }
    }
}
