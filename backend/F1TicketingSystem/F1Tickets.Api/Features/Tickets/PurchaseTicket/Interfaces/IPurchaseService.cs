using F1Tickets.Api.Features.Tickets.PurchaseTicket.DTOs;

namespace F1Tickets.Api.Features.Tickets.PurchaseTicket.Interfaces
{
    public interface IPurchaseService
    {
        Task<PurchaseResponseDto> PurchaseTicketsAsync(PurchaseRequestDto dto);
    }
}
