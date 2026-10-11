namespace F1Tickets.Worker.Messaging.Listeners
{
    public interface IRabbitMqWorkerListener
    {
        void Subscribe<T>(string queueName, Func<T, Task> onMessage) where T : class;
    }
}
