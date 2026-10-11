using F1Tickets.Api.Events.DeleteTicket;
using F1Tickets.Api.Events.PurchaseTicket;
using F1Tickets.Api.Events.UpdateTicket;
using F1Tickets.Worker.Consumers;
using F1Tickets.Worker.Messaging.Listeners;

namespace F1Tickets.Worker.Background
{
    public class WorkerBackground : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRabbitMqWorkerListener _listener;

        public WorkerBackground(IServiceScopeFactory scopeFactory, IRabbitMqWorkerListener listener)
        {
            _scopeFactory = scopeFactory;
            _listener = listener;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1 Subsctibe to Purchase Queue
            _listener.Subscribe<TicketPurchaseRequestedEvent>(
                "tickets.worker.purchase.queue",
                async evt =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<TicketPurchaseConsumer>();
                    await consumer.HandleAsync(evt);
                });

            // 2 Subscribe to Update Queue
            _listener.Subscribe<TicketUpdateRequestedEvent>(
                "tickets.worker.update.queue",
                async evt =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<TicketUpdateConsumer>();
                    await consumer.HandleAsync(evt);
                });

            // 3 Subscribe to Delete Queue
            _listener.Subscribe<TicketDeleteRequestedEvent>(
                "tickets.worker.delete.queue",
                async evt =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var consumer = scope.ServiceProvider.GetRequiredService<TicketDeleteConsumer>();
                    await consumer.HandleAsync(evt);
                });

            // 4 Keep Worker alive
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}