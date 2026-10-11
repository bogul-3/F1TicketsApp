using F1Tickets.Api.Features.RaceDays.DTOs;

namespace F1Tickets.Api.Features.RaceEvents.DTOs
{
    public class RaceEventFullDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string City { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<RaceDayDtoForRaceEvent> RaceDays { get; set; } 
    }
}
