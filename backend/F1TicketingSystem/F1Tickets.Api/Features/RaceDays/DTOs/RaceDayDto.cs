namespace F1Tickets.Api.Features.RaceDays.DTOs
{
    public class RaceDayDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public decimal BasePrice { get; set; }
        public Guid RaceEventId { get; set; }
    }
}
