using System.ComponentModel.DataAnnotations;

namespace F1Tickets.Api.Features.Tickets.DeleteTicket.DTOs
{
    public class TicketDeleteRequestDto
    {
        [Required]
        public Guid TicketId { get; set; }

        [Required]
        public string AccessCode { get; set; } 

        [Required]
        [EmailAddress]
        public string Email { get; set; } 
    }
}
