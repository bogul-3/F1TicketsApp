using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.ZoneDays.DTOs;
using F1Tickets.Api.Features.ZoneDays.Interfaces;
using F1Tickets.Api.Infrastructure.Cache;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Api.Features.ZoneDays.Services
{
    public class ZoneDayService : IZoneDayService
    {
        private readonly F1TicketsDbContext _db;
        private readonly RedisCacheService _cache;

        public ZoneDayService(F1TicketsDbContext db, RedisCacheService cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<Guid> CreateZoneDayAsync(CreateZoneDayDto dto)
        {
            // 1 Check if RaceDay and Zone exist
            var raceDay = await _db.RaceDays.FindAsync(dto.RaceDayId);
            if (raceDay == null)
                throw new Exception("RaceDay not found");

            var zone = await _db.Zones.FindAsync(dto.ZoneId);
            if (zone == null)
                throw new Exception("Zone not found");

            // 2 Check if ZoneDay Exists
            var exists = await _db.ZoneDays.AnyAsync(x => x.ZoneId == dto.ZoneId && x.RaceDayId == dto.RaceDayId);
            if (exists) throw new Exception("Zone is already assigned to this RaceDay");

            // 3 Validate Capacity and PriceAddon
            if (dto.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0");
            if (dto.PriceAddon < 0)
                throw new ArgumentException("PriceAddon cannot be negative");

            // 4 Create ZoneDay
            var zoneDay = new ZoneDay
            {
                Id = Guid.NewGuid(),
                ZoneId = dto.ZoneId,
                RaceDayId = dto.RaceDayId,
                Capacity = dto.Capacity,
                SeatsLeft = dto.Capacity,
                PriceAddon = dto.PriceAddon,
            };

            // 5 Insert ZoneDay into db
            _db.ZoneDays.Add(zoneDay);
            await _db.SaveChangesAsync();

            // 6 Invalidate Cache
            await _cache.KeyDeleteAsync("zonedays:all");
            await _cache.KeyDeleteAsync($"zonedays:raceday:{dto.RaceDayId}");

            return zoneDay.Id;
        }

        public async Task DeleteZoneDayAsync(Guid id)
        {
            // 1 Check if ZoneDay exists
            var zoneDay = await _db.ZoneDays.FindAsync(id);
            if (zoneDay == null)
                throw new Exception("zoneDay not found");

            // 2 Delete from db
            _db.ZoneDays.Remove(zoneDay);
            await _db.SaveChangesAsync();

            // 3 Invalidate Cache
            await _cache.KeyDeleteAsync("zonedays:all");
            await _cache.KeyDeleteAsync($"zonedays:raceday:{zoneDay.RaceDayId}");
        }

        public async Task<List<ZoneDayDto>> GetAllAsync()
        {
            // 1 Create CacheKey
            var cacheKey = "zonedays:all";

            // 2 Try Get from Cache
            var cached = await _cache.GetAsync<List<ZoneDayDto>>(cacheKey);
            if (cached != null)
            {
                Console.WriteLine("Returned from Redis cache");
                return cached;
            }

            // 3 Get from db
            var list = await _db.ZoneDays
                .Select(x => new ZoneDayDto
                {
                    Id = x.Id,
                    ZoneId = x.ZoneId,
                    RaceDayId = x.RaceDayId,
                    Capacity = x.Capacity,
                    SeatsLeft = x.SeatsLeft,
                    PriceAddon = x.PriceAddon
                })
                .ToListAsync();

            // 4 Set ZoneDays into Cache
            await _cache.SetAsync(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<ZoneDayDto?> GetByIdAsync(Guid id)
        {
            // 1 Create CacheKey
            var cacheKey = $"zoneday:{id}";

            // 2 Try Get from Cache
            var cached = await _cache.GetAsync<ZoneDayDto>(cacheKey);
            if (cached != null)
            {
                Console.WriteLine("Returned from Redis cache");
                return cached;
            }

            // 3 Get from db
            var dto = await _db.ZoneDays
                .Where(x => x.Id == id)
                .Select(x => new ZoneDayDto
                {
                    Id = x.Id,
                    ZoneId = x.ZoneId,
                    RaceDayId = x.RaceDayId,
                    Capacity = x.Capacity,
                    SeatsLeft = x.SeatsLeft,
                    PriceAddon = x.PriceAddon
                })
                .FirstOrDefaultAsync();

            // 4 Set ZoneDay into Cache
            if (dto != null)
                await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10));

            return dto;
        }

        public async Task<List<ZoneDayDto>> GetByRaceDayAsync(Guid raceDayId)
        {
            // 1 Create CacheKey
            var cacheKey = $"zonedays:raceday:{raceDayId}";

            // 2 Try Get from Cache
            var cached = await _cache.GetAsync<List<ZoneDayDto>>(cacheKey);
            if (cached != null)
            {
                Console.WriteLine("Returned from Redis cache");
                return cached;
            }

            // 3 Get from db
            var list = await _db.ZoneDays
                .Where(x => x.RaceDayId == raceDayId)
                .Select(x => new ZoneDayDto
                {
                    Id = x.Id,
                    ZoneId = x.ZoneId,
                    RaceDayId = x.RaceDayId,
                    ZoneName=x.Zone.Name,
                    Capacity = x.Capacity,
                    SeatsLeft = x.SeatsLeft,
                    PriceAddon = x.PriceAddon
                })
                .ToListAsync();

            // 4 Set ZoneDays into Cache
            await _cache.SetAsync(cacheKey, list, TimeSpan.FromMinutes(10));
            return list;
        }

        public async Task<bool> UpdateZoneDayAsync(Guid zoneDayId, UpdateZoneDayDto dto)
        {
            // 1 Check if ZoneDay exists
            var zoneDay = await _db.ZoneDays
                .Include(x => x.RaceDay)
                .Include(x => x.Zone)
                .FirstOrDefaultAsync(x => x.Id == zoneDayId);
            if (zoneDay == null)
                return false;

            // 2 Validate Capacity and PriceAddon
            if (dto.Capacity <= 0)
                throw new ArgumentException("Capacity must be greater than 0");

            if (dto.PriceAddon < 0)
                throw new ArgumentException("PriceAddon cannot be negative");

            // 3 Update ZoneDay Fields
            zoneDay.Capacity = dto.Capacity;
            zoneDay.PriceAddon = dto.PriceAddon;
            if(zoneDay.SeatsLeft > dto.Capacity)
                zoneDay.SeatsLeft = dto.Capacity;

            // 4 Insert into db
            await _db.SaveChangesAsync();

            // 5 Invalidate Cache
            await _cache.KeyDeleteAsync("zonedays:all");
            await _cache.KeyDeleteAsync($"zonedays:raceday:{zoneDay.RaceDayId}");
            await _cache.KeyDeleteAsync($"zoneday:{zoneDay.Id}");

            return true;
        }
    }
}