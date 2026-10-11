using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.RaceDays.DTOs;
using F1Tickets.Api.Features.RaceDays.Interfaces;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace F1Tickets.Api.Features.RaceDays.Services
{
    public class RaceDayService : IRaceDayService
    {
        private readonly F1TicketsDbContext _db;
        public RaceDayService(F1TicketsDbContext db)
        {
            _db = db;
        }
        public async Task<Guid> CreateRaceDayAsync(CreateRaceDayDto dto)
        {
            // 1 Check if RaceDay exists
            var raceEventExists = await _db.RaceEvents.AnyAsync(x => x.Id == dto.RaceEventId);
            if (!raceEventExists)
                throw new Exception("RaceEvent not found");

            // 2 Fetch RaceEvent from db
            var raceEvent = await _db.RaceEvents.FirstOrDefaultAsync(x => x.Id == dto.RaceEventId);
            if (raceEvent == null)
                throw new Exception("RaceEvent not found");

            // 3 Check if the date is within the RaceEvent range
            var dateUtc = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);
            if (dateUtc < raceEvent.StartDate || dateUtc > raceEvent.EndDate)
                throw new Exception("RaceDay date must be within the RaceEvent duration");

            // 4 BasePrice validation
            if (dto.BasePrice <= 0)
                throw new Exception("BasePrice has to be > 0");

            // 5 Create RaceDay
            var raceDay = new RaceDay
            {
                Id = Guid.NewGuid(),
                Date = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc),
                BasePrice = dto.BasePrice,
                RaceEventId = dto.RaceEventId
            };

            // 6 Insert RaceDay into db
            _db.RaceDays.Add(raceDay);
            await _db.SaveChangesAsync();

            return raceDay.Id;
        }

        public async Task DeleteRaceDayAsync(Guid id)
        {
            // 1 Check if RaceDay exists
            var raceDay = await _db.RaceDays.FindAsync(id);
            if (raceDay == null)
                throw new Exception("RaceDay not found");

            // 2 Delete from db
            _db.RaceDays.Remove(raceDay);
            await _db.SaveChangesAsync();
        }

        public async Task<RaceDayDto?> GetByIdAsync(Guid id)
        {
            // 1 Get RaceDay by id
            return await _db.RaceDays
                .Where(x => x.Id == id)
                .Select(x => new RaceDayDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    BasePrice = x.BasePrice,
                    RaceEventId = x.RaceEventId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<RaceDayDto>> GetByRaceEventAsync(Guid raceEventId)
        {
            // 1 Get RaceDays by RaceEvent
            return await _db.RaceDays
                .Where(x => x.RaceEventId == raceEventId)
                .Select(x => new RaceDayDto
                {
                    Id = x.Id,
                    Date = x.Date,
                    BasePrice = x.BasePrice,
                    RaceEventId = x.RaceEventId
                })
                .ToListAsync();
        }

        public async Task UpdateRaceDayAsync(Guid id, UpdateRaceDayDto dto)
        {
            // 1 Check if RaceDay exists
            var raceDay = await _db.RaceDays
                .Include(rd => rd.RaceEvent)
                .FirstOrDefaultAsync(rd => rd.Id == id);
            if (raceDay == null)
                throw new Exception("RaceDay not found");

            // 2 Check if the new date is within the RaceEvent range
            var dateUtc = DateTime.SpecifyKind(dto.Date, DateTimeKind.Utc);
            if (dateUtc < raceDay.RaceEvent.StartDate || dateUtc > raceDay.RaceEvent.EndDate)
                throw new Exception("RaceDay date must be within the RaceEvent duration");

            // 3 Update RaceDay fields
            raceDay.Date = dto.Date;
            raceDay.BasePrice = dto.BasePrice;

            // 4 Insert into db
            _db.RaceDays.Update(raceDay);
            await _db.SaveChangesAsync();
        }
    }
}
