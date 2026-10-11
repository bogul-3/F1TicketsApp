using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.RaceEvents.DTOs;
using F1Tickets.Api.Features.ZoneDays.DTOs;

namespace F1Tickets.Api.Features.RaceEvents.Interfaces
{
    public interface IRaceEventService
    {
        Task<RaceEvent> CreateRaceEventAsync(CreateRaceEventDto dto);
        Task<List<RaceEvent>> GetAllAsync();
        Task<ZoneDayDto?> GetByIdAsync(Guid id);
        Task<RaceEventFullDto?> GetFullEventAsync(Guid eventId);
        Task UpdateRaceEventAsync(Guid id, UpdateRaceEventDto dto);
        Task DeleteRaceEventAsync (Guid id);
        Task<List<RaceEventFullDto>> GetAllFullRAceEventsAsync();
    }
}
