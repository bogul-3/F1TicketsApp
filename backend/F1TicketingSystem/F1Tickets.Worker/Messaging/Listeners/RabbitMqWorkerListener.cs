using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace F1Tickets.Worker.Messaging.Listeners
{
    public class RabbitMqWorkerListener : IRabbitMqWorkerListener, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqWorkerListener(string hostName = "localhost")
        {
            Console.WriteLine("[Listener] Constructor called");

            // 1 Create ConnectionFactory
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

            // 2 Create Connection
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // 3 Declare Exchange
            _channel.ExchangeDeclare("tickets.exchange", ExchangeType.Direct, durable: true);

            // 4 Bind to Purchase Queue
            _channel.QueueDeclare(
                queue: "tickets.worker.purchase.queue",
                durable: true,
                exclusive: false,
                autoDelete: false);
            _channel.QueueBind(
                queue: "tickets.worker.purchase.queue",
                exchange: "tickets.exchange",
                routingKey: "TicketPurchaseRequestEvent");

            // 5 Bind to Update Queue
            _channel.QueueDeclare(
                queue: "tickets.worker.update.queue",
                durable: true,
                exclusive: false,
                autoDelete: false);
            _channel.QueueBind(
                queue: "tickets.worker.update.queue",
                exchange: "tickets.exchange",
                routingKey: "TicketUpdateRequestEvent");

            // 6 Bind to Delete Queue
            _channel.QueueDeclare(
                queue: "tickets.worker.delete.queue",
                durable: true,
                exclusive: false,
                autoDelete: false);
            _channel.QueueBind(
                queue: "tickets.worker.delete.queue",
                exchange: "tickets.exchange",
                routingKey: "TicketDeleteRequestEvent");
        }

        public void Subscribe<T>(string queueName, Func<T, Task> onMessage) where T : class
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                // 1 Deserialize message
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<T>(json);
                if (message == null)
                {
                    Console.WriteLine("[Listener] Deserialization FAILED!");
                    return;
                }

                // 2 Process message
                await onMessage(message);

                // 3 Return Ack
                _channel.BasicAck(ea.DeliveryTag, false);
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
