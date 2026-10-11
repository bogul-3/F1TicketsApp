public interface IRabbitMqWorkerPublisher
{
    Task PublishAsync<T>(T message);
}
