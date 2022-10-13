using EventBus.Base.Abstraction;
using EventBus.UnitTest.Events.Events;

namespace EventBus.UnitTest.Events.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        Console.WriteLine("Handle method worked with id: " + @event.Id);
        return Task.CompletedTask;
    }
}