using F1Tickets.Api.Features.Zones.DTOs;
using System;

namespace F1Tickets.Api.Features.Tickets.DTOs
{
    public class TicketDto
    {
        public Guid TicketId { get; set; }           // Id same karte
        public string EventName { get; set; }       // Naziv eventa iz ZoneDay -> RaceDay -> Event
        public DateTime RaceDay { get; set; }        // Datum trke
        public string ZoneName { get; set; }      // Naziv zone iz ZoneDay
        public int Quantity { get; set; }            // Broj kupljenih karata
        public decimal TotalPrice { get; set; }      // Ukupna cena
        public string CurrencyCode { get; set; } // EUR/USD/RSD
        public string Status { get; set; } // TicketStatus
        public string AccessCode { get; set; } // Ako treba

        public List<ZoneDayDtoForUserTicket> AvailableZones { get; set; } 
    }

    public class ZoneDayDtoForUserTicket
    {
        public Guid Id { get; set; }          // Id ZoneDay-a
        public Guid ZoneId { get; set; }      // Id zone (npr. VIP, Standard)
        public string ZoneName { get; set; }   // Naziv zone (za dropdown prikaz)
        public Guid RaceDayId { get; set; }   // Id dana trke kojem pripada zona
        public int Capacity { get; set; }     // Ukupna kapacitet zone
        public int SeatsLeft { get; set; }    // Preostala mesta
        public decimal PriceAddon { get; set; } // Dodatna cena za zonu
    }
}