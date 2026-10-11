using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.Zones.DTOs;
using F1Tickets.Api.Features.Zones.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Tickets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonesController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZonesController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        [HttpGet]
        public async Task<List<ZoneDto>> GetAll()
        {
            return await _zoneService.GetAllZonesAsync();
        }

        [HttpGet("{id}")]
        public async Task<ZoneDto> GetById(Guid id)
        {
            var result = await _zoneService.GetZoneByIdAsync(id);

            if (result == null) 
                throw new Exception("Zone not found");

            return result;
        }

        [HttpPost]
        public async Task<Guid> Create(string name)
        {
            return await _zoneService.CreateZoneAsync(name);
        }

        [HttpPut("{id}")]
        public async Task Update(Guid id, string name)
        {
            try
            {
                await _zoneService.UpdateZoneAsync(id, name);
            }
            catch (ArgumentException ex)
            {
                BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                BadRequest
                    (new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            await _zoneService.DeleteZoneAsync(id);
        }
    }
}