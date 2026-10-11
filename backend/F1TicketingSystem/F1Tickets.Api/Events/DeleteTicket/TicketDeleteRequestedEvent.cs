namespace F1Tickets.Api.Events.DeleteTicket
{
    public class TicketDeleteRequestedEvent
    {
        public Guid TicketId { get; set; }

        public string AccessCode { get; set; } 

        public string Email { get; set; } 
    }
}
