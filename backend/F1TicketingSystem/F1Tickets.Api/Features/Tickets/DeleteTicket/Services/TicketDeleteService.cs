using F1Tickets.Api.Events.DeleteTicket;
using F1Tickets.Api.Features.Tickets.DeleteTicket.DTOs;
using F1Tickets.Api.Features.Tickets.DeleteTicket.Interfaces;
using F1Tickets.Api.Infrastructure.Messaging;

namespace F1Tickets.Api.Features.Tickets.DeleteTicket.Services
{
    public class TicketDeleteService : ITicketDeleteService
    {
        private readonly IRabbitMqApiPublisher _publisher;

        public TicketDeleteService(IRabbitMqApiPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task DeleteTicketAsync(TicketDeleteRequestDto dto)
        {
            // 1 Create Delete Request Event
            var evt = new TicketDeleteRequestedEvent
            {
                TicketId = dto.TicketId,
                AccessCode = dto.AccessCode,
                Email = dto.Email,
            };

            // 2 Publish Delete Request Event
            await _publisher.PublishTicketDeleteAsync(evt);
        }
    }
}
