using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace F1Tickets.Worker.Messaging.Publishers
{
    public class RabbitMqWorkerPublisher : IRabbitMqWorkerPublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _exchange;

        public RabbitMqWorkerPublisher(IConfiguration config)
        {
            // 1 Create Connection Factory
            var factory = new ConnectionFactory()
            {
                HostName = config["RabbitMQ:Host"] ?? "localhost",
                UserName = config["RabbitMQ:Username"] ?? "guest",
                Password = config["RabbitMQ:Password"] ?? "guest",
                Port = int.Parse(config["RabbitMQ:Port"] ?? "5672"),
                VirtualHost = config["RabbitMQ:VirtualHost"] ?? "/",
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
            };

            // 2 Create Connection
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // 3 Set exchange
            _exchange = config["RabbitMQ:ReportingExchange"] ?? "tickets.reporting.exchange";

            // 4 Declare exchange
            _channel.ExchangeDeclare(_exchange, ExchangeType.Direct, durable: true);
        }

        public Task PublishAsync<T>(T message)
        {
            // 1 Serialize Message
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // 2 Save Message after RabbitMQ restarts
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;

            // 3 Publish message
            _channel.BasicPublish(
                exchange: _exchange,
                routingKey: typeof(T).Name,
                basicProperties: properties,
                body: body
            );
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            Console.WriteLine("[Publisher] Disposed");
        }
    }
}