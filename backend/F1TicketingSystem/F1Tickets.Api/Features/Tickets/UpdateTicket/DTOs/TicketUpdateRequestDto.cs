using System.ComponentModel.DataAnnotations;

namespace F1Tickets.Api.Features.Tickets.UpdateTicket.DTOs
{
    public class TicketUpdateRequestDto
    {
        [Required]
        public Guid TicketId { get; set; }

        [Required]
        public string AccessCode { get; set; } 

        [Required]
        [EmailAddress]
        public string Email { get; set; } 

        public Guid AddZoneDayId { get; set; }
        public Guid RemoveZoneDayId { get; set; }
    }
}