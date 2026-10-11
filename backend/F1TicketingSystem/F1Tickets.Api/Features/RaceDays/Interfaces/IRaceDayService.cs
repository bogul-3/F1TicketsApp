using F1Tickets.Api.Features.RaceDays.DTOs;

namespace F1Tickets.Api.Features.RaceDays.Interfaces
{
    public interface IRaceDayService
    {
        Task<Guid> CreateRaceDayAsync(CreateRaceDayDto dto);
        Task<List<RaceDayDto>> GetByRaceEventAsync(Guid raceEventId);
        Task<RaceDayDto?> GetByIdAsync(Guid id);
        Task UpdateRaceDayAsync(Guid id, UpdateRaceDayDto dto);
        Task DeleteRaceDayAsync(Guid id);
    }
}
