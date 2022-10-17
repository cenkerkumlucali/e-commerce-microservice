using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers;

public class OrderPaymentSuccessIntegrationEventHandler:IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
{
    private readonly ILogger<OrderPaymentSuccessIntegrationEvent> _logger;

    public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEvent> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        //Send fail Notification (Sms, Email, Push);
        _logger.LogInformation($"Order Payment success with OrderId: {@event.OrderId}");
        return Task.CompletedTask;
    }
}