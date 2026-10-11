using F1Tickets.Api.Events.UpdateTicket;
using F1Tickets.Api.Features.Tickets.UpdateTicket.DTOs;
using F1Tickets.Api.Features.Tickets.UpdateTicket.Interfaces;
using F1Tickets.Api.Infrastructure.Messaging;

namespace F1Tickets.Api.Features.Tickets.UpdateTicket.Services
{
    public class TicketUpdateService : ITicketUpdateService
    {
        private readonly IRabbitMqApiPublisher _publisher;

        public TicketUpdateService(IRabbitMqApiPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task UpdateTicketAsync(TicketUpdateRequestDto dto)
        {
            // 1 Check if zonedays are the same
            if(dto.AddZoneDayId == dto.RemoveZoneDayId)
                return;

            // 2 Create Update Request Event
            var updateEvent = new TicketUpdateRequestedEvent
            {
                TicketId = dto.TicketId,
                AccessCode = dto.AccessCode,
                Email = dto.Email,
                AddZoneDayId = dto.AddZoneDayId,
                RemoveZoneDayId = dto.RemoveZoneDayId
            };

            // 3 Publish Update Request Event
            await _publisher.PublishTicketUpdateAsync(updateEvent);
        }
    }
}