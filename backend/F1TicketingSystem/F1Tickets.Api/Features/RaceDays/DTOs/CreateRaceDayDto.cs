namespace F1Tickets.Api.Features.RaceDays.DTOs
{
    public class CreateRaceDayDto
    {
        public DateTime Date { get; set; }
        public decimal BasePrice { get; set; }
        public Guid RaceEventId { get; set; }
    }
}
