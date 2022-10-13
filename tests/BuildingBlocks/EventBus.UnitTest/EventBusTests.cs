using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using EventBus.UnitTest.Events.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EventBus.UnitTest;

public class EventBusTests
{
    private ServiceCollection _serviceCollection;

    public EventBusTests()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddLogging(configure => configure.AddConsole());
    }

    [Fact]
    public void subscribe_event_on_rabbitmq_test()
    {
        _serviceCollection.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(),sp);
        });
        var sp = _serviceCollection.BuildServiceProvider();
        var eventBus = sp.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandler>();
        // eventBus.UnSubscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandler>();
    }

    [Fact]
    public void send_message_to_rabbitmq()
    {
        _serviceCollection.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(),sp);
        });
        var sp = _serviceCollection.BuildServiceProvider();
        var eventBus = sp.GetRequiredService<IEventBus>();
        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    private EventBusConfig GetRabbitMQConfig()
    {
       return new EventBusConfig()
        {   
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "EventBus.UniTest",
            DefaultTopicName = "CkTopicName",
            EventBusType = EventBusType.RabbitMQ,
            EventNameSuffix = "IntegrationEvent",
            // Connection = new ConnectionFactory()
            // {
            //     HostName = "localhost",
            //     Port = 15672,
            //     UserName = "guest",
            //     Password = "guest"
            // }
        };
    }
}