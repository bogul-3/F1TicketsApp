using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace F1Tickets.Reporting.Api.Infrastructure.Messaging
{
    public class RabbitMqReportingListener : IRabbitMqReportingListener, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqReportingListener(
            string hostName = "localhost",
            string exchangeName = "tickets.reporting.exchange")
        {
            // 1 Create Connection Factory
            var factory = new ConnectionFactory()
            {
                HostName = hostName,
                UserName = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
                DispatchConsumersAsync = true,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            // 2 Create Conection
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // 3 Declare Exchange
            _channel.ExchangeDeclare("tickets.reporting.exchange", ExchangeType.Direct, durable: true);

            // 4 Bind to Reporting Purchase Queue
            _channel.QueueDeclare("tickets.reporting.purchased.queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(
                queue: "tickets.reporting.purchased.queue",
                exchange: "tickets.reporting.exchange",
                routingKey: "TicketPurchasedEvent");

            // 5 Bind to Reporting Update Queue
            _channel.QueueDeclare("tickets.reporting.updated.queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(
                queue: "tickets.reporting.updated.queue",
                exchange: "tickets.reporting.exchange",
                routingKey: "TicketUpdatedEvent");

            // 6 Bind to Reporting Delete Queue
            _channel.QueueDeclare("tickets.reporting.deleted.queue", durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(
                queue: "tickets.reporting.deleted.queue",
                exchange: "tickets.reporting.exchange",
                routingKey: "TicketDeletedEvent");
        }

        public void Subscribe<T>(string queueName, Func<T, Task> onMessage) where T : class
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (sender, ea) =>
            {
                try
                {
                    // 1 Deserialize message
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var message = JsonSerializer.Deserialize<T>(json);

                    // 2 Process message
                    if (message != null)
                        await onMessage(message);

                    // 3 return Ack
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}