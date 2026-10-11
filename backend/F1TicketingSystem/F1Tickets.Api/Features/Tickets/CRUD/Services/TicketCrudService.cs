using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.Tickets.CRUD.Interfaces;
using F1Tickets.Api.Features.Tickets.DTOs;
using F1Tickets.Api.Features.ZoneDays.Interfaces;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Api.Features.Tickets.CRUD.Services
{
    public class TicketCrudService : ITicketCrudService
    {
        private readonly F1TicketsDbContext _db;
        private readonly IZoneDayService _service;
        public TicketCrudService(F1TicketsDbContext db, IZoneDayService service)
        {
            _db = db;
            _service = service;
        }
        public async Task<List<TicketDto>> GetTicketsByUserAsync(Guid userId)
        {
            // Dohvatanje tiketa korisnika
            var tickets = await _db.Tickets
                .Include(t => t.ZoneDay)
                    .ThenInclude(zd => zd.Zone)
                .Include(t => t.ZoneDay)
                    .ThenInclude(zd => zd.RaceDay)
                        .ThenInclude(rd => rd.RaceEvent)
                .Where(t => t.UserId == userId)
                .ToListAsync();

            var ticketDtos = new List<TicketDto>();

            foreach (var t in tickets)
            {
                // Dohvati sve dostupne zone za taj RaceDay
                var availableZones = await _service.GetByRaceDayAsync(t.ZoneDay.RaceDayId);

                var ticketDto = new TicketDto
                {
                    TicketId = t.Id,
                    EventName = t.ZoneDay.RaceDay.RaceEvent.Name,
                    RaceDay = t.ZoneDay.RaceDay.Date,
                    ZoneName = t.ZoneDay.Zone.Name,
                    Quantity = 1, // ako čuvaš quantity u Ticket entity, promeni ovde
                    TotalPrice = t.TotalPrice,
                    CurrencyCode = t.CurrencyCode,
                    Status = t.Status.ToString(),
                    AccessCode = t.AccessCode,
                    AvailableZones = availableZones.Select(zd => new ZoneDayDtoForUserTicket
                    {
                        Id = zd.Id,
                        ZoneId = zd.ZoneId,
                        ZoneName = zd.ZoneName,
                        RaceDayId = zd.RaceDayId,
                        Capacity = zd.Capacity,
                        SeatsLeft = zd.SeatsLeft,
                        PriceAddon = zd.PriceAddon
                    }).ToList()
                };

                ticketDtos.Add(ticketDto);
            }

            return ticketDtos;
        }
    }
}
