using F1Tickets.Api.Events.DeleteTicket;
using F1Tickets.Api.Events.PurchaseTicket;
using F1Tickets.Api.Events.UpdateTicket;
using F1Tickets.Reporting.Api.Consumers;
using F1Tickets.Reporting.Api.Infrastructure.Messaging;

namespace F1Tickets.Reporting.Api.Background
{
    public class ReportingBackground : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRabbitMqReportingListener _listener;

        public ReportingBackground(IServiceScopeFactory scopeFactory, IRabbitMqReportingListener listener)
        {
            _scopeFactory = scopeFactory;
            _listener = listener;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1 Subscribe to Reporting Purchase Queue
            _listener.Subscribe<TicketPurchasedEvent>(
                "tickets.reporting.purchased.queue",
                async evt =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<ReportingTicketPurchasedConsumer>();
                    await consumer.HandleAsync(evt);
                });

            // 2 Subscribe to Reporting Update Queue
            _listener.Subscribe<TicketUpdatedEvent>(
                "tickets.reporting.updated.queue",
                async evt =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<ReportingTicketUpdatedConsumer>();
                    await consumer.HandleAsync(evt);
                });

            // 3 Subscribe to Reporting Deleted Queue
            _listener.Subscribe<TicketDeletedEvent>(
                "tickets.reporting.deleted.queue",
                async evt =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<ReportingTicketDeletedConsumer>();
                    await consumer.HandleAsync(evt);
                });

            return Task.CompletedTask;
        }
    }
}