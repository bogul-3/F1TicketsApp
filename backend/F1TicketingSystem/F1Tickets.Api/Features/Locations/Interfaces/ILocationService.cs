using F1Tickets.Api.Features.Locations.DTOs;

namespace F1Tickets.Api.Features.Locations.Interfaces
{
    public interface ILocationService
    {
        Task<List<CountryDto>> GetAllCountriesAsync();
        Task<List<CityDto>> GetCitiesByCountryAsync(Guid countryId);
    }
}