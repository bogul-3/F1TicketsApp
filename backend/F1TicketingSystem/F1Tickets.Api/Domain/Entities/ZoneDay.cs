using F1Tickets.Api.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace F1Tickets.Api.Domain.Entities
{
    public class ZoneDay : BaseEntity
    {
        public int SeatsLeft { get; set; }
        public int Capacity { get; set; }
        public decimal PriceAddon { get; set; }

        // RELATIONSHIPS

        public Guid RaceDayId { get; set; }
        public RaceDay RaceDay { get; set; } 

        public Guid ZoneId { get; set; }
        public Zone Zone { get; set; } 

        public List<Ticket> Tickets { get; set; } 
    }
}