using F1Tickets.Api.Domain.Entities;
using F1Tickets.Api.Features.Tickets.DTOs;

namespace F1Tickets.Api.Features.Tickets.CRUD.Interfaces
{
    public interface ITicketCrudService
    {
        Task<List<TicketDto>> GetTicketsByUserAsync(Guid userId);
    }
}
