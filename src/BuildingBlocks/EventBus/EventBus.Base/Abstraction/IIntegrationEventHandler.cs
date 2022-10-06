using EventBus.Base.Event;

namespace EventBus.Base.Abstraction;

public interface IIntegrationEventHandler<TIntegrationEvent> : IntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}

public interface IntegrationEventHandler
{
}