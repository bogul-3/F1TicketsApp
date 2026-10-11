using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.RaceDays.DTOs;
using F1Tickets.Api.Features.RaceEvents.DTOs;
using F1Tickets.Api.Features.RaceEvents.Interfaces;
using F1Tickets.Api.Features.ZoneDays.DTOs;
using F1Tickets.Api.Infrastructure.Cache;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace F1Tickets.Api.Features.RaceEvents.Services
{
    public class RaceEventService : IRaceEventService
    {
        private readonly F1TicketsDbContext _db;
        private readonly RedisCacheService _cache;

        public RaceEventService(F1TicketsDbContext db, RedisCacheService cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<RaceEvent> CreateRaceEventAsync(CreateRaceEventDto dto)
        {
            // 1 Find City for RaceEvent
            var city = await _db.Cities.FindAsync(dto.CityId);
            if (city == null)
                throw new ArgumentException("CityId not found");

            // 2 dates validation
            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("StartDate cannot be after EndDate");

            if (dto.DiscountValidUntil > dto.StartDate)
                throw new ArgumentException("DiscountValidUntil cannot be after StartDate");

            if (dto.StartDate < DateTime.UtcNow)
                throw new ArgumentException("StartDate cannot be in the past");

            // 3 Name validation
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required");

            var exists = await _db.RaceEvents
                .AnyAsync(re => re.Name == dto.Name && re.CityId == dto.CityId &&
                                re.StartDate.Date == dto.StartDate.Date);
            if (exists)
                throw new ArgumentException("RaceEvent with same name and date already exists in this city");

            // 4 Create RaceEvent
            var raceEvent = new RaceEvent
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc),
                AdditionalInfo = dto.AdditionalInfo,
                DiscountValidUntil = DateTime.SpecifyKind(dto.DiscountValidUntil, DateTimeKind.Utc),
                CityId = dto.CityId,
                City = city
            };

            // 5 Insert RaceEvent into db
            _db.RaceEvents.Add(raceEvent);
            await _db.SaveChangesAsync();

            // 6 Invalidate all RaceEvents list cache
            await _cache.KeyDeleteAsync("raceevents:all");

            return raceEvent;
        }

        public async Task<ZoneDayDto?> GetByIdAsync(Guid id)
        {
            // 1 Get ZoneDay by id
            return await _db.ZoneDays.Where(x => x.Id == id).Select(x => new ZoneDayDto { Id = x.Id, ZoneId = x.ZoneId, RaceDayId = x.RaceDayId, Capacity = x.Capacity, SeatsLeft = x.SeatsLeft, PriceAddon = x.PriceAddon }).FirstOrDefaultAsync();
        }

        public async Task<List<RaceEvent>> GetAllAsync()
        {
            // 1 Create CacheKey
            var cacheKey = "raceevents:all";

            // 2 Try Get from Cache
            var cached = await _cache.GetAsync<List<RaceEvent>>(cacheKey);
            if (cached != null)
            {
                Console.WriteLine("Returned from Redis cache");
                return cached;
            }

            // 3 Get from db
            var events = await _db.RaceEvents
                .Include(x => x.City)
                .ToListAsync();

            // 4 Set RaceEvents into Cache 
            await _cache.SetAsync(cacheKey, events, TimeSpan.FromMinutes(10));
            return events;
        }

        public async Task<RaceEventFullDto?> GetFullEventAsync(Guid eventId)
        {
            // 1 Create CacheKey
            var cacheKey = $"raceeventfull:{eventId}";

            // 2 Try Get from Cache
            var cached = await _cache.GetAsync<RaceEventFullDto>(cacheKey);
            if (cached != null)
            {
                Console.WriteLine("Returned from Redis cache");
                return cached;
            }

            // 3 Get from db
            var evt = await _db.RaceEvents
                .Include(x => x.City)
                .Include(e => e.RaceDays)
                    .ThenInclude(d => d.ZoneDays)
                        .ThenInclude(z => z.Zone)
                .FirstOrDefaultAsync(e => e.Id == eventId);
            if (evt == null)
                return null;

            // 4 Create ResultDto
            var result = new RaceEventFullDto
            {
                Id = evt.Id,
                Name = evt.Name,
                City = evt.City.Name,
                StartDate = evt.StartDate,
                EndDate = evt.EndDate,

                RaceDays = evt.RaceDays.Select(day => new RaceDayDtoForRaceEvent
                {
                    Id = day.Id,
                    Date = day.Date,
                    BasePrice = day.BasePrice,

                    ZoneDays = day.ZoneDays.Select(z => new ZoneDayDtoForRaceEvent
                    {
                        ZoneDayId = z.Id,
                        Capacity = z.Capacity,
                        SeatsLeft = z.SeatsLeft,
                        PriceAddon = z.PriceAddon,

                        Zone = new ZoneDtoForRaceEvent
                        {
                            Id = z.Zone.Id,
                            Name = z.Zone.Name
                        }
                    }).ToList()
                }).ToList()
            };

            // 5 Set RaceEvent into Cache
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }

        public async Task UpdateRaceEventAsync(Guid id, UpdateRaceEventDto dto)
        {
            // 1 Check if RaceEvent exists
            var raceEvent = await _db.RaceEvents.FindAsync(id);
            if (raceEvent == null)
                throw new Exception("RaceEvent not found.");

            // 2 Validate fields
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required");

            if (dto.StartDate > dto.EndDate)
                throw new ArgumentException("StartDate cannot be after EndDate");

            if (dto.DiscountValidUntil > dto.StartDate)
                throw new ArgumentException("DiscountValidUntil cannot be after StartDate");

            if (dto.StartDate < DateTime.UtcNow)
                throw new ArgumentException("StartDate cannot be in the past");

            if (!string.IsNullOrEmpty(dto.AdditionalInfo) && dto.AdditionalInfo.Length > 500)
                throw new ArgumentException("AdditionalInfo is too long");

            // Check for duplicate RaceEvent in the same city on the same date
            var exists = await _db.RaceEvents
                .AnyAsync(re => re.Id != id &&
                                re.Name == dto.Name &&
                                re.CityId == dto.CityId &&
                                re.StartDate.Date == dto.StartDate.Date);
            if (exists)
                throw new ArgumentException("Another RaceEvent with the same name and date already exists in this city");

            // 3 Update RaceEvent fields
            raceEvent.Name = dto.Name;
            raceEvent.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
            raceEvent.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);
            raceEvent.AdditionalInfo = dto.AdditionalInfo;
            raceEvent.DiscountValidUntil = DateTime.SpecifyKind(dto.DiscountValidUntil, DateTimeKind.Utc);
            raceEvent.CityId = dto.CityId;

            // 4 Save changes
            _db.RaceEvents.Update(raceEvent);
            await _db.SaveChangesAsync();

            // 5 Invalidate Cache
            if (_cache != null)
            {
                await _cache.KeyDeleteAsync($"raceeventfull:{id}");
                await _cache.KeyDeleteAsync("raceevents:all");
            }
        }

        public async Task DeleteRaceEventAsync(Guid id)
        {
            // 1 Check if raceEvent exists
            var raceEvent = await _db.RaceEvents.FindAsync(id);
            if (raceEvent == null) throw new Exception("RaceEvent not found");

            // 2 Delete from db
            _db.RaceEvents.Remove(raceEvent);
            await _db.SaveChangesAsync();

            // 3 Invalidate Cache
            if (_cache != null)
            {
                await _cache.KeyDeleteAsync($"raceeventfull:{id}");
                await _cache.KeyDeleteAsync("raceevents:all");
            }
        }

        public async Task<List<RaceEventFullDto>> GetAllFullRAceEventsAsync()
        {
            // 1 Create CacheKey
            var cacheKey = $"raceeventsfull";

            // 2 Try Get from Cache
            var cached = await _cache.GetAsync<List<RaceEventFullDto>>(cacheKey);
            if (cached != null)
            {
                Console.WriteLine("Returned from Redis cache");
                return cached;
            }

            // 3 Get from db
            var evt = await _db.RaceEvents
                .Include(x => x.City)
                .Include(e => e.RaceDays)
                    .ThenInclude(d => d.ZoneDays)
                        .ThenInclude(z => z.Zone)
                .ToListAsync();
            if (evt == null)
                return null;

            // 4 Create ResultDto LIST
            var result = evt.Select(e => new RaceEventFullDto
            {
                Id = e.Id,
                Name = e.Name,
                City = e.City.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,

                RaceDays = e.RaceDays.Select(day => new RaceDayDtoForRaceEvent
                {
                    Id = day.Id,
                    Date = day.Date,
                    BasePrice = day.BasePrice,

                    ZoneDays = day.ZoneDays.Select(z => new ZoneDayDtoForRaceEvent
                    {
                        ZoneDayId = z.Id,
                        Capacity = z.Capacity,
                        SeatsLeft = z.SeatsLeft,
                        PriceAddon = z.PriceAddon,
                    }).ToList()

                }).ToList()

            }).ToList();

            // 5 Set RaceEvents into Cache
            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

            return result;
        }
    }
}
