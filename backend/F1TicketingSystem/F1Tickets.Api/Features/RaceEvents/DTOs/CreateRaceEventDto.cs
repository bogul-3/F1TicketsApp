using System.ComponentModel.DataAnnotations;

namespace F1Tickets.Api.Features.RaceEvents.DTOs
{
    public class CreateRaceEventDto
    {
        [Required]
        public string Name { get; set; } 

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string AdditionalInfo { get; set; } 

        public DateTime DiscountValidUntil { get; set; }

        [Required]
        public Guid CityId { get; set; }
    }
}
