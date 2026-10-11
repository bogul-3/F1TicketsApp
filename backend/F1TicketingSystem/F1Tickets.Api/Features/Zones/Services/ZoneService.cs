using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.Zones.DTOs;
using F1Tickets.Api.Features.Zones.Interfaces;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Api.Features.Zones.Services
{
    public class ZoneService : IZoneService
    {
        private readonly F1TicketsDbContext _db;

        public ZoneService(F1TicketsDbContext db)
        {
            _db = db;
        }

        public async Task<List<ZoneDto>> GetAllZonesAsync()
        {
            // 1 Get Zones From Db
            return await _db.Zones
                .Select(z => new ZoneDto
                {
                    Id = z.Id,
                    Name = z.Name,
                    ZoneDays = z.ZoneDays.Select(zd => new ZoneDayDto
                    {
                        Id = zd.Id,
                        Capacity = zd.Capacity,
                        SeatsLeft = zd.SeatsLeft,
                        PriceAddon = zd.PriceAddon
                    }).ToList()
                }).ToListAsync();
        }


        public async Task<ZoneDto?> GetZoneByIdAsync(Guid id)
        {
            // 1 Get Zone By id
            return await _db.Zones
                .Where(z => z.Id == id)
                .Select(z => new ZoneDto
                {
                    Id = z.Id,
                    Name = z.Name,
                    ZoneDays = z.ZoneDays.Select(zd => new ZoneDayDto
                    {
                        Id = zd.Id,
                        Capacity = zd.Capacity,
                        SeatsLeft = zd.SeatsLeft,
                        PriceAddon = zd.PriceAddon
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Guid> CreateZoneAsync(string name)
        {
            // 1 Validate name
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Zone name is required");

            if (name.Length > 40)
                throw new ArgumentException("Zone name is too long (max 40 characters)");

            // 2 Check if Zone with same name already exists
            var exists = await _db.Zones.AnyAsync(z => z.Name == name);
            if (exists)
                throw new ArgumentException("Zone with the same name already exists");

            // 3 Create Zone
            Zone zone = new Zone()
            {
                Id = Guid.NewGuid(),
                Name = name,
            };

            // 4 Insert Zone into db
            _db.Zones.Add(zone);
            await _db.SaveChangesAsync();

            return zone.Id;
        }

        public async Task UpdateZoneAsync(Guid id, string name)
        {
            // 1 Check if Zone exists
            var existing = await _db.Zones.FindAsync(id);
            if (existing == null)
                throw new Exception("Zone not found");

            // 2 Validate name
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Zone name is required");

            if (name.Length > 40)
                throw new ArgumentException("Zone name is too long (max 40 characters)");

            // 3 Check if another Zone with the same name exists
            var duplicate = await _db.Zones
                .AnyAsync(z => z.Id != id && z.Name == name);
            if (duplicate)
                throw new ArgumentException("Another Zone with the same name already exists");

            // 4 Update Zone Fields
            existing.Name = name;

            // 3 Insert into db
            await _db.SaveChangesAsync();
        }

        public async Task DeleteZoneAsync(Guid id)
        {
            // 1 Check if Zone exists
            var zone = await _db.Zones.FindAsync(id);
            if (zone == null) throw new Exception("Zone not found");

            // 2 Delete Zone from db
            _db.Zones.Remove(zone);
            await _db.SaveChangesAsync();
        }
    }
}