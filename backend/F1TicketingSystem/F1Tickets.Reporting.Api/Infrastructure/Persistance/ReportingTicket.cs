using F1Tickets.Api.Domain.Enums;

namespace F1Tickets.Reporting.Api.Infrastructure.Persistance
{
    public class ReportingTicket
    {
        public Guid Id { get; set; }

        public Guid TicketId { get; set; }
        public Guid userId { get; set; }
        public Guid ZoneDayId { get; set; }
        public Guid RaceEventId { get; set; }
        public Guid RaceDayId { get; set; }

        public DateTime PurchasedOn { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public TicketStatus Status { get; set; }
    }
}
