using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Events;

namespace NotificationService.IntegrationEvents.EventHandlers;

public class OrderPaymentFailedIntegrationEventHandler:IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    private readonly ILogger<OrderPaymentFailedIntegrationEvent> _logger;

    public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEvent> logger)
    {
        _logger = logger;
    }

    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        //Send fail Notification (Sms, Email, Push);
        _logger.LogInformation($"Order Payment failed with OrderId: {@event.OrderId}, ErrorMessage: {@event.ErrorMessage}");
        return Task.CompletedTask;
    }
}