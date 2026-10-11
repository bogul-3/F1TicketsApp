using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace F1Tickets.Api.Infrastructure.Messaging
{
    public class RabbitMqApiPublisher : IRabbitMqApiPublisher
    {
        private readonly IModel _channel;
        private readonly string _exchange;
        public RabbitMqApiPublisher(IModel channel, IConfiguration config)
        {
           _channel = channel;
           _exchange = config["RabbitMQ:Exchange"] ?? "tickets.exchange";

           _channel.ExchangeDeclare(_exchange, ExchangeType.Direct, durable: true);
        }
        public Task PublishPurchaseAsync<T>(T message)
        {
            // 1 Serialize message
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            // 2 Publish message
            _channel.BasicPublish(
                exchange: _exchange,
                routingKey: "TicketPurchaseRequestEvent",
                basicProperties: null,
                body: body);

            Console.WriteLine($"[Publisher] Published event to exchange {_exchange}: {json}");
            return Task.CompletedTask;
        }

        public Task PublishTicketDeleteAsync<T>(T @event) where T : class
        {
            // 1 Serialize event
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            // 2 Publish message
            _channel.BasicPublish(
                exchange: _exchange,
                routingKey: "TicketDeleteRequestEvent",
                basicProperties: null,
                body: body);

            Console.WriteLine($"[Publisher] Published TicketDelete event to exchange {_exchange}: {json}");
            return Task.CompletedTask;
        }

        public Task PublishTicketUpdateAsync<T>(T @event) where T : class
        {
            // 1 Serialize event
            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            // 2 Publish message
            _channel.BasicPublish(
                exchange: _exchange,
                routingKey: "TicketUpdateRequestEvent",
                basicProperties: null,
                body: body);

            Console.WriteLine($"[Publisher] Published TicketUpdate event to exchange {_exchange}: {json}");
            return Task.CompletedTask;
        }
    }
}
