using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.RaceDays.DTOs;
using F1Tickets.Api.Features.RaceDays.Interfaces;
using F1Tickets.Api.Features.RaceDays.Services;
using F1Tickets.Api.Features.RaceEvents.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace F1Tickets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceDaysController : ControllerBase
    {
        private readonly IRaceDayService _service;

        public RaceDaysController(IRaceDayService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceDayDto dto)
        {
            try
            {
                await _service.CreateRaceDayAsync(dto);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("by-event/{raceEventId}")]
        public async Task<IActionResult> GetByRaceEvent(Guid raceEventId)
        {
            return Ok(await _service.GetByRaceEventAsync(raceEventId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if(result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteRaceDayAsync(id);
                return Ok(new { message = "RaceEvent deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRaceDayDto dto)
        {
            try
            {
                await _service.UpdateRaceDayAsync(id, dto);
                return Ok(new { message = "RaceEvent updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
