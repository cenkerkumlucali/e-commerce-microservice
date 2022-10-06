using EventBus.Base.Event;

namespace EventBus.Base.Abstraction;

public interface IEventBusSubscriptionManager
{
    bool IsEmpty { get; }
    event EventHandler<string> OnEventRemoved;
    void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    bool HasSubscriptionForEvent<T>() where T : IntegrationEvent;
    bool HasSubscriptionForEvent(string eventName);
    Type GetEventTypeByName(string eventName);
    void Clear();
    IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
    IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
    string GetEventKey<T>();
}