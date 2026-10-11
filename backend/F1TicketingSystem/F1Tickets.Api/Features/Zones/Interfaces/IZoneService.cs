using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.Zones.DTOs;

namespace F1Tickets.Api.Features.Zones.Interfaces
{
    public interface IZoneService
    {
        Task<List<ZoneDto>> GetAllZonesAsync();
        Task<ZoneDto?> GetZoneByIdAsync(Guid id);
        Task<Guid> CreateZoneAsync(string name);
        Task UpdateZoneAsync(Guid id,string name);
        Task DeleteZoneAsync(Guid id);
    }
}