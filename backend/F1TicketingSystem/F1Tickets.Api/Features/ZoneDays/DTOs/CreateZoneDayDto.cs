namespace F1Tickets.Api.Features.ZoneDays.DTOs
{
    public class CreateZoneDayDto
    {
        public Guid ZoneId { get; set; }
        public Guid RaceDayId { get; set; }
        public int Capacity { get; set; }
        public decimal PriceAddon { get; set; }
    }
}
