using F1Tickets.Api.Features.ZoneDays.DTOs;
using F1Tickets.Api.Features.ZoneDays.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Tickets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZoneDaysController : ControllerBase 
    {
        private readonly IZoneDayService _service;
        public ZoneDaysController(IZoneDayService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateZoneDayDto dto)
        {
            var id = await _service.CreateZoneDayAsync(dto);
            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if(result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("raceday/{raceDayId}")]
        public async Task<IActionResult> GetByRaceDay(Guid raceDayId)
        {
            var result = await _service.GetByRaceDayAsync(raceDayId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateZoneDay(Guid id, UpdateZoneDayDto dto)
        {
            var success = await _service.UpdateZoneDayAsync(id, dto);

            if (!success)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task <IActionResult> DeleteZoneDay(Guid id)
        {
            try
            {
                await _service.DeleteZoneDayAsync(id);
                return Ok(new { message = "RaceEvent deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
