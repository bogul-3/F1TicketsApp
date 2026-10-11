using F1Tickets.Api.Features.ZoneDays.DTOs;

namespace F1Tickets.Api.Features.RaceDays.DTOs
{
    public class RaceDayDtoForRaceEvent
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal BasePrice { get; set; }

        public List<ZoneDayDtoForRaceEvent> ZoneDays { get; set; } 
    }
}
