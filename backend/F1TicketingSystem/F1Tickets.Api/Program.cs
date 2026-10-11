using F1Tickets.Api.Features.Auth.Interfaces;
using F1Tickets.Api.Features.Auth.Services;
using F1Tickets.Api.Features.Currencies.Interfaces;
using F1Tickets.Api.Features.Currencies.Services;
using F1Tickets.Api.Features.Locations.Interfaces;
using F1Tickets.Api.Features.Locations.Services;
using F1Tickets.Api.Features.RaceDays.Interfaces;
using F1Tickets.Api.Features.RaceDays.Services;
using F1Tickets.Api.Features.RaceEvents.Interfaces;
using F1Tickets.Api.Features.RaceEvents.Services;
using F1Tickets.Api.Features.Tickets.CRUD.Interfaces;
using F1Tickets.Api.Features.Tickets.CRUD.Services;
using F1Tickets.Api.Features.Tickets.DeleteTicket.Interfaces;
using F1Tickets.Api.Features.Tickets.DeleteTicket.Services;
using F1Tickets.Api.Features.Tickets.PurchaseTicket.Interfaces;
using F1Tickets.Api.Features.Tickets.UpdateTicket.Interfaces;
using F1Tickets.Api.Features.Tickets.UpdateTicket.Services;
using F1Tickets.Api.Features.ZoneDays.Interfaces;
using F1Tickets.Api.Features.ZoneDays.Services;
using F1Tickets.Api.Features.Zones.Interfaces;
using F1Tickets.Api.Features.Zones.Services;
using F1Tickets.Api.Infrastructure.Cache;
using F1Tickets.Api.Infrastructure.Messaging;
using F1Tickets.Api.Infrastructure.Persistance;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

// 0 Build WebApplication 
var builder = WebApplication.CreateBuilder(args);

// 1 PostgreSQL
var ticketsConnString = "Host=localhost;Port=5433;Database=f1tickets;Username=postgres;Password=postgres";
builder.Services.AddDbContext<F1TicketsDbContext>(opt => opt.UseNpgsql(ticketsConnString));

// 2 Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration["Redis:ConnectionString"];

    if (string.IsNullOrEmpty(config))
        throw new Exception("Redis connection string is not configured.");

    return ConnectionMultiplexer.Connect(config);
});

// 3 RabbitMQ
builder.Services.AddSingleton<ConnectionFactory>(sp => new ConnectionFactory()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest"
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = sp.GetRequiredService<ConnectionFactory>();
    return factory.CreateConnection();
});

builder.Services.AddSingleton<IModel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();
    return connection.CreateModel();
});
builder.Services.AddSingleton<IRabbitMqApiPublisher, RabbitMqApiPublisher>();


// 4 Register Services
builder.Services.AddScoped<IRaceEventService, RaceEventService>();
builder.Services.AddScoped<IRaceDayService, RaceDayService>();
builder.Services.AddScoped<IZoneDayService, ZoneDayService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ITicketUpdateService, TicketUpdateService>();
builder.Services.AddScoped<ITicketDeleteService, TicketDeleteService>();
builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddHttpClient<ICurrencyConversionService, CurrencyConversionService>();
builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ITicketCrudService, TicketCrudService>();

// 5 Web API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 6 JWT Endpoint protection
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
    });

// 7 Add Cors for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();