using F1Tickets.Api.Features.Locations.DTOs;
using F1Tickets.Api.Features.Locations.Interfaces;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace F1Tickets.Api.Features.Locations.Services
{
    public class LocationService : ILocationService
    {
        private readonly F1TicketsDbContext _context;

        public LocationService(F1TicketsDbContext context)
        {
            _context = context;
        }

        public async Task<List<CountryDto>> GetAllCountriesAsync()
        {
            return await _context.Countries
                .Select(c => new CountryDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }

        public async Task<List<CityDto>> GetCitiesByCountryAsync(Guid countryId)
        {
            return await _context.Cities
                .Where(c => c.CountryId == countryId)
                .Select(c => new CityDto { Id = c.Id, Name = c.Name, CountryId = c.CountryId })
                .ToListAsync();
        }
    }
}