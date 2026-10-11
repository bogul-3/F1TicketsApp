using F1Tickets.Api.Features.Tickets.CRUD.Interfaces;
using F1Tickets.Api.Features.Tickets.DeleteTicket.DTOs;
using F1Tickets.Api.Features.Tickets.DeleteTicket.Interfaces;
using F1Tickets.Api.Features.Tickets.DTOs;
using F1Tickets.Api.Features.Tickets.PurchaseTicket.DTOs;
using F1Tickets.Api.Features.Tickets.PurchaseTicket.Interfaces;
using F1Tickets.Api.Features.Tickets.UpdateTicket.DTOs;
using F1Tickets.Api.Features.Tickets.UpdateTicket.Interfaces;
using F1Tickets.Api.Infrastructure.Messaging;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TicketController : ControllerBase
{
    private readonly IPurchaseService _ticketPurchaseService;
    private readonly IRabbitMqApiPublisher _publisher;
    private readonly ITicketUpdateService _ticketUpdateService;
    private readonly ITicketDeleteService _ticketDeleteService;
    private readonly ITicketCrudService _ticketCrudService;

    public TicketController(IPurchaseService service, IRabbitMqApiPublisher publisher, ITicketUpdateService ticketUpdateService, ITicketDeleteService ticketDeleteService, ITicketCrudService ticketCrudService)
    {
        _ticketPurchaseService = service;
        _publisher = publisher;
        _ticketUpdateService = ticketUpdateService;
        _ticketDeleteService = ticketDeleteService;
        _ticketCrudService = ticketCrudService;
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequestDto dto)
    {
        try
        {
            var result = await _ticketPurchaseService.PurchaseTicketsAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateTicket([FromBody] TicketUpdateRequestDto dto)
    {
        try
        {
            await _ticketUpdateService.UpdateTicketAsync(dto);

            return Accepted(new
            {
                message = "Ticket update request sent for processing."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteTicket([FromBody] TicketDeleteRequestDto dto)
    {
        await _ticketDeleteService.DeleteTicketAsync(dto);

        return Ok(new
        {
            message = "Ticket delete request sent"
        });
    }

    [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<List<TicketDto>>> GetTicketsByUser(Guid userId)
    {
        var tickets = await _ticketCrudService.GetTicketsByUserAsync(userId);
        return Ok(tickets);
    }
}