using F1Tickets.Api.Features.Tickets.UpdateTicket.DTOs;

namespace F1Tickets.Api.Features.Tickets.UpdateTicket.Interfaces
{
    public interface ITicketUpdateService
    {
        Task UpdateTicketAsync(TicketUpdateRequestDto dto);
    }
}
