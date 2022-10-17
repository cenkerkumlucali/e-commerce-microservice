using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.EventHandlers;
using NotificationService.IntegrationEvents.Events;

ServiceCollection serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var sp = serviceCollection.BuildServiceProvider(); 
IEventBus eventBus = sp.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();

Console.WriteLine("Application is Running....");
Console.ReadLine();

static void ConfigureServices(ServiceCollection services)
{
    services.AddLogging(configure => configure.AddConsole());
    services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
    services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

    services.AddSingleton<IEventBus>(sp =>
        EventBusFactory.Create(new()
        {
            ConnectionRetryCount = 5,
            EventNameSuffix = "IntegrationEvent",
            SubscriberClientAppName = "NotificationService",
            EventBusType = EventBusType.RabbitMQ
        }, sp)
    );
}