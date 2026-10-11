namespace F1Tickets.Api.Features.ZoneDays.DTOs
{
    public class ZoneDayDtoForRaceEvent
    {
        public Guid ZoneDayId { get; set; }
        public int Capacity { get; set; }
        public int SeatsLeft { get; set; }
        public decimal PriceAddon { get; set; }
        public ZoneDtoForRaceEvent Zone { get; set; }
    }

    public class ZoneDtoForRaceEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
