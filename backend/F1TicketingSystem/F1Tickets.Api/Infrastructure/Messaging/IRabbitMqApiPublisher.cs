namespace F1Tickets.Api.Infrastructure.Messaging
{
    public interface IRabbitMqApiPublisher
    {
        Task PublishPurchaseAsync<T>(T message);
        Task PublishTicketUpdateAsync<T>(T @event) where T : class;
        Task PublishTicketDeleteAsync<T>(T @event) where T : class;
    }
}
