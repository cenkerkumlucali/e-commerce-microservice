using EventBus.Base.Event;

namespace OrderService.Application.IntegrationEvents;

public class OrderStartedIntegrationEvent : IntegrationEvent
{
    public string UserName { get; set; }

    public OrderStartedIntegrationEvent(string userName)
    {
        UserName = userName;
    }
}