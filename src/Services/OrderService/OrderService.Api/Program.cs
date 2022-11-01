using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using OrderService.Api.Extensions;
using OrderService.Api.Extensions.Registration.EventHandlerRegistration;
using OrderService.Api.Extensions.Registration.ServiceDiscovery;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

static IHost BuildWebHost(IConfiguration configuration, string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseDefaultServiceProvider((context, options) => { options.ValidateOnBuild = false; })
        .ConfigureAppConfiguration(i => i.AddConfiguration(configuration))
        .ConfigureWebHostDefaults(webBuilder => { }).Build();

static IConfiguration GetConfiguration()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddEnvironmentVariables();

    return builder.Build();
}

builder.Services.AddLogging(configure => configure.AddConsole())
    .AddApplicationRegistration(typeof(Program))
    .AddPersistenceRegistration(builder.Configuration)
    .ConfigureEventHandlers()
    .ConfigureConsul(builder.Configuration);

builder.Services.AddSingleton<IEventBus>(sp =>
    EventBusFactory.Create(new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "OrderService",
        EventBusType = EventBusType.RabbitMQ
    }, sp));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MigrateDbContext<OrderDbContext>((context, services) =>
{
    var logger = services.GetService<ILogger<OrderDbContextSeed>>();
    var dbContextSeeder = new OrderDbContextSeed();
    dbContextSeeder.SeedAsync(context, logger).GetAwaiter();
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Start();

app.RegisterWithConsul(app.Lifetime);
ConfigureEventBusForSubscription(app);

app.WaitForShutdown();


void ConfigureEventBusForSubscription(IApplicationBuilder applicationBuilder)
{
    var eventBus = applicationBuilder.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}