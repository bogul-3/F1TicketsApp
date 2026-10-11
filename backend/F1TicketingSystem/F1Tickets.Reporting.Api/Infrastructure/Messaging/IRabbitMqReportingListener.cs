namespace F1Tickets.Reporting.Api.Infrastructure.Messaging
{
    public interface IRabbitMqReportingListener
    {
        void Subscribe<T>(string queueName, Func<T, Task> onMessage) where T : class;
    }
}
