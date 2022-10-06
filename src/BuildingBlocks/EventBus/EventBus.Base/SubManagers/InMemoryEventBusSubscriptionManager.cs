using EventBus.Base.Abstraction;
using EventBus.Base.Event;

namespace EventBus.Base.SubManagers;

public class InMemoryEventBusSubscriptionManager:IEventBusSubscriptionManager
{
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
    private readonly List<Type> _eventTypes;

  

    public event EventHandler<string>? OnEventRemoved;
    public Func<string, string> eventNameGetter;
    
    public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
        this.eventNameGetter = eventNameGetter;
    }


    public bool IsEmpty => !_handlers.Keys.Any();
    public void Clear() => _handlers.Clear();
    
    public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventKey<T>();
        AddSubscription(typeof(TH),eventName);
        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }
    }
    private void AddSubscription(Type handlerType,string eventName)
    {
        if (!HasSubscriptionForEvent(eventName))
        {
            _handlers.Add(eventName,new List<SubscriptionInfo>());
        }

        if (_handlers[eventName].Any(c => c.HandleType == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'",
                nameof(handlerType));
        }
        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
    }

    public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        SubscriptionInfo handlerToRemove = FindSubscriptionToRemove<T, TH>();
        string eventName = GetEventKey<T>();
        RemoveHandler(eventName,handlerToRemove);
    }

    private void RemoveHandler(string eventName, SubscriptionInfo subsToRemove)
    {
        if (subsToRemove is not null)
        {
            _handlers[eventName].Remove(subsToRemove);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                Type? eventType = _eventTypes.SingleOrDefault(c => c.Name == eventName);
                if (eventType is not null) _eventTypes.Remove(eventType);

                RaiseOnEventRemoved(eventName);
            }
        }
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        EventHandler<string>? handler = OnEventRemoved;
        handler?.Invoke(this,eventName);
    }

    private SubscriptionInfo FindSubscriptionToRemove<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        string eventName = GetEventKey<T>();
        return FindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo FindSubscriptionToRemove(string eventName, Type type)
    {
        if (!HasSubscriptionForEvent(eventName))
        {
            return null;
        }

        return _handlers[eventName].SingleOrDefault(c => c.HandleType == type);
    }

    public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
    {
        string key = GetEventKey<T>();
        return HasSubscriptionForEvent(key);
    }

    public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);

    public Type GetEventTypeByName(string eventName) => _eventTypes.SingleOrDefault(c => c.Name == eventName);

    

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        string key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName) => _handlers[eventName];

    public string GetEventKey<T>()
    {
        string eventName = typeof(T).Name;
        return eventNameGetter(eventName);
    }
}