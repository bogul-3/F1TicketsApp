using F1Tickets.Api.Features.ZoneDays.DTOs;

namespace F1Tickets.Api.Features.ZoneDays.Interfaces
{
    public interface IZoneDayService
    {
        Task<Guid> CreateZoneDayAsync(CreateZoneDayDto dto);
        Task<List<ZoneDayDto>> GetAllAsync();
        Task<ZoneDayDto?> GetByIdAsync(Guid id);
        Task<List<ZoneDayDto>> GetByRaceDayAsync(Guid raceDayId);
        Task<bool> UpdateZoneDayAsync(Guid zoneDayId, UpdateZoneDayDto dto);
        Task DeleteZoneDayAsync(Guid id);
    }
}
