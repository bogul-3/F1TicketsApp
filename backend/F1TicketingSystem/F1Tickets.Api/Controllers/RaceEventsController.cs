using F1Tickets.Api.Features.RaceEvents.DTOs;
using F1Tickets.Api.Features.RaceEvents.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace F1Tickets.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RaceEventsController : ControllerBase
    {
        private readonly IRaceEventService _service;

        public RaceEventsController(IRaceEventService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRaceEventDto dto)
        {
            var result = await _service.CreateRaceEventAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}/full")]
        public async Task<IActionResult> GetFullEvent(Guid id)
        {
            var result = await _service.GetFullEventAsync(id);

            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("full")]
        public async Task<IActionResult> GetFullEventList()
        {
            var result = await _service.GetAllFullRAceEventsAsync();
            if(result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRaceEventDto dto)
        {
            try
            {
                await _service.UpdateRaceEventAsync(id, dto);
                return Ok(new { message = "RaceEvent updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteRaceEventAsync(id);
                return Ok(new { message = "RaceEvent deleted successfully" });
            } catch (Exception ex) {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
