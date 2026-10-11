using F1Tickets.Api.Features.Tickets.DeleteTicket.DTOs;

namespace F1Tickets.Api.Features.Tickets.DeleteTicket.Interfaces
{
    public interface ITicketDeleteService
    {
        public Task DeleteTicketAsync(TicketDeleteRequestDto dto);
    }
}
