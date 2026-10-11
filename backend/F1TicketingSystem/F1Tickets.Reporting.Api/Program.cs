using F1Tickets.Reporting.Api.Background;
using F1Tickets.Reporting.Api.Consumers;
using F1Tickets.Reporting.Api.Infrastructure.Messaging;
using F1Tickets.Reporting.Api.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1 Controllers / Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2 DbContext
builder.Services.AddDbContext<F1ReportingDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5433;Database=f1tickets_reporting;Username=postgres;Password=postgres"));

// 3 Consumers
builder.Services.AddScoped<ReportingTicketPurchasedConsumer>();
builder.Services.AddScoped<ReportingTicketUpdatedConsumer>();
builder.Services.AddScoped<ReportingTicketDeletedConsumer>();

// 4 Listener
builder.Services.AddSingleton<IRabbitMqReportingListener, RabbitMqReportingListener>();

// 5 Background service
builder.Services.AddHostedService<ReportingBackground>();

// 6 Add Cors for frontend
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

// 7 HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();