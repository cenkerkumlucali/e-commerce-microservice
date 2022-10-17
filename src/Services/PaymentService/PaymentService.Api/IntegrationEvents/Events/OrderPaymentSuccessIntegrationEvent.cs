using EventBus.Base.Event;

namespace PaymentService.Api.IntegrationEvents.Events;

public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }

    public OrderPaymentSuccessIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}