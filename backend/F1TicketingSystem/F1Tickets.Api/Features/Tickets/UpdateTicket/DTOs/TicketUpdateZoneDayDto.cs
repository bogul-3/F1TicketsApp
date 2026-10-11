namespace F1Tickets.Api.Features.Tickets.UpdateTicket.DTOs
{
    public class TicketUpdateZoneDayDto
    {
        public Guid ZoneDayId { get; set; }
        public int SeatsLeft { get; set; }
        public decimal PriceAddon { get; set; }
        public Guid RaceDayId { get; set; }
        public Guid ZoneId { get; set; }
    }
}
