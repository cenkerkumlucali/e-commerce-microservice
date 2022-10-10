using EventBus.Base.Event;

namespace EventBus.UnitTest.Events.Events;

public class OrderCreatedIntegrationEvent:IntegrationEvent
{
    public int Id { get; set; }

    public OrderCreatedIntegrationEvent(int id)
    {
        Id = id;
    }
}