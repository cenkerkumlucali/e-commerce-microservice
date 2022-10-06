using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Event;

public abstract class BaseEventBus : IEventBus
{
    public readonly IServiceProvider ServiceProvider;
    public readonly IEventBusSubscriptionManager SubscriptionManager;
    private EventBusConfig _eventBusConfig;

    protected BaseEventBus(EventBusConfig config, IServiceProvider serviceProvider)
    {
        _eventBusConfig = config;
        ServiceProvider = serviceProvider;
        SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
    }

    public virtual string ProcessEventName(string eventName)
    {
        if (_eventBusConfig.DeleteEventPrefix)
            eventName = eventName.TrimStart(_eventBusConfig.EventNamePrefix.ToArray());
        if (_eventBusConfig.DeleteEventSuffix)
            eventName = eventName.TrimEnd(_eventBusConfig.EventNameSuffix.ToArray());
        return eventName;
    }

    public virtual string GetSubName(string eventName)
    {
        return $"{_eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
    }


    public async Task<bool> ProcessEvent(string eventName, string message)
    {
        eventName = ProcessEventName(eventName);
        bool processed = false;
        if (SubscriptionManager.HasSubscriptionForEvent(eventName))
        {
            var subscriptions = SubscriptionManager.GetHandlersForEvent(eventName);
            using (var scope = ServiceProvider.CreateScope())
            {
                foreach (var subscription in subscriptions)
                {
                    var handler = ServiceProvider.GetService(subscription.HandleType);
                    if (handler is null) continue;
                    var eventType = SubscriptionManager.GetEventTypeByName(
                        $"{_eventBusConfig.EventNamePrefix}{eventName}{_eventBusConfig.EventNameSuffix}");
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object?[] { integrationEvent });
                }
            }

            processed = true;
        }

        return processed;
    }

    public virtual void Dispose()
    {
        _eventBusConfig = null;
    }
    
    public abstract void Publish(IntegrationEvent @event);

    public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

    public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
}