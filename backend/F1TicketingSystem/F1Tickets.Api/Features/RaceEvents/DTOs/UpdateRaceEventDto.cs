namespace F1Tickets.Api.Features.RaceEvents.DTOs
{
    public class UpdateRaceEventDto
    {
        public string Name { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string AdditionalInfo { get; set; } 
        public DateTime DiscountValidUntil { get; set; }
        public Guid CityId { get; set; }
    }
}
