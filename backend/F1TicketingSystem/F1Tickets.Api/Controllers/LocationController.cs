using F1Tickets.Api.Features.Locations.DTOs;
using F1Tickets.Api.Features.Locations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Tickets.Api.Features.Locations
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("countries")]
        public async Task<ActionResult<List<CountryDto>>> GetAllCountries()
        {
            var countries = await _locationService.GetAllCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("cities/{countryId}")]
        public async Task<ActionResult<List<CityDto>>> GetCitiesByCountry(Guid countryId)
        {
            var cities = await _locationService.GetCitiesByCountryAsync(countryId);
            return Ok(cities);
        }
    }
}