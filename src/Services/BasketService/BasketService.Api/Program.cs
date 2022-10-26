using BasketService.Api.Core.Application.Repositories;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using BasketService.Api.Core.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
ConfigureServicesExt(builder.Services);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasketService.Api v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Start();

app.RegisterWithConsul(app.Lifetime);
ConfigureSubscription(app.Services);

app.WaitForShutdown();

void ConfigureServicesExt(IServiceCollection services)
{
    services.ConfigureAuth(builder.Configuration);
    services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));
    services.ConfigureConsul(builder.Configuration);
    services.AddHttpContextAccessor();
    services.AddTransient<IBasketRepository, RedisBasketRepository>();
    services.AddTransient<IIdentityService, BasketService.Api.Core.Application.Services.IdentityService>();
    

    services.AddSingleton<IEventBus>(sp =>
        EventBusFactory.Create(new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "BasketService",
            EventBusType = EventBusType.RabbitMQ
        }, sp)
    );

    services.AddTransient<OrderCreatedIntegrationEventHandler>();
}

void ConfigureSubscription(IServiceProvider serviceProvider)
{
    var eventBus = serviceProvider.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
}