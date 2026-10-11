using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;
using F1Tickets.Api.Infrastructure.Persistance;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly IModel _rabbitChannel;
    private readonly IConnectionMultiplexer _redis;
    private readonly F1TicketsDbContext _ticketsDb;

    public TestController(
        IModel rabbitChannel,
        IConnectionMultiplexer redis,
        F1TicketsDbContext ticketsDb)
    {
        _rabbitChannel = rabbitChannel;
        _redis = redis;
        _ticketsDb = ticketsDb;
    }

    // --- 1. Test RabbitMQ ---
    [HttpPost("send-rabbitmq")]
    public IActionResult SendRabbitMessage()
    {
        var message = "Test poruka sa API kontrolera";
        var body = Encoding.UTF8.GetBytes(message);

        var props = _rabbitChannel.CreateBasicProperties();
        props.Persistent = true;

        _rabbitChannel.BasicPublish(
            exchange: "tickets.exchange",
            routingKey: "tickets.create",
            basicProperties: props,
            body: body
        );

        return Ok("RabbitMQ poruka poslata!");
    }

    // --- 2. Test Redis ---
    [HttpPost("test-redis")]
    public IActionResult TestRedis()
    {
        var db = _redis.GetDatabase();
        db.StringSet("testkey", "testvalue");
        var value = db.StringGet("testkey");

        return Ok($"Redis test: {value}");
    }

    // --- 3. Test PostgreSQL ---
    [HttpGet("test-db")]
    public async Task<IActionResult> TestDb()
    {
        try
        {
            // Proveravamo da li se može otvoriti konekcija
            var canConnect = await _ticketsDb.Database.CanConnectAsync();
            return Ok($"PostgreSQL konekcija: {canConnect}");
        }
        catch (System.Exception ex)
        {
            return BadRequest($"Greška pri konekciji: {ex.Message}");
        }
    }
}
