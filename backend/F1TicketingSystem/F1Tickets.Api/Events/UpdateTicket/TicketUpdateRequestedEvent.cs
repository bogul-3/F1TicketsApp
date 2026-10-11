namespace F1Tickets.Api.Events.UpdateTicket
{
    public class TicketUpdateRequestedEvent
    {
        public Guid TicketId { get; set; }
        public string AccessCode { get; set; } 
        public string Email { get; set; } 
        public Guid AddZoneDayId { get; set; }
        public Guid RemoveZoneDayId { get; set; }
    }
}