using F1Tickets.Api.Infrastructure.Cache;
using F1Tickets.Api.Infrastructure.Persistance;
using F1Tickets.Worker.Background;
using F1Tickets.Worker.Consumers;
using F1Tickets.Worker.Messaging.Listeners;
using F1Tickets.Worker.Messaging.Publishers;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// 1 DbContext
builder.Services.AddDbContext<F1TicketsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("F1TicketsDatabase") ??
                      "Host=localhost;Port=5433;Database=f1tickets;Username=postgres;Password=postgres"));

// 2 RabbitMQ
builder.Services.AddSingleton<IRabbitMqWorkerListener, RabbitMqWorkerListener>();
builder.Services.AddSingleton<IRabbitMqWorkerPublisher, RabbitMqWorkerPublisher>();

// 3 Scoped Consumers
builder.Services.AddScoped<TicketPurchaseConsumer>();
builder.Services.AddScoped<TicketUpdateConsumer>();
builder.Services.AddScoped<TicketDeleteConsumer>();

// 4 Background Worker
builder.Services.AddHostedService<WorkerBackground>();

// 5 Redis Cache
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration["Redis:ConnectionString"];

    if (string.IsNullOrEmpty(config))
        throw new Exception("Redis connection string is not configured.");

    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddSingleton<RedisCacheService>();

// SERVICES

var app = builder.Build();

Console.WriteLine("[Worker] Starting Worker host...");
await app.RunAsync();