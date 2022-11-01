using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application.Features.Commands.CreateOrder;

namespace OrderService.Api.IntegrationEvents.EventHandlers;

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedIntegrationEventHandler> _logger;

    public OrderCreatedIntegrationEventHandler(IMediator mediator, ILogger<OrderCreatedIntegrationEventHandler> logger)
    {
        this._mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedIntegrationEvent @event)
    {
        _logger.LogInformation("Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})",
            @event.Id,
            typeof(Program).Namespace,
            @event);

        var createOrderCommand = new CreateOrderCommand(@event.Basket.Items, @event.UserName, @event.City,
            @event.Street, @event.State, @event.Country, @event.ZipCode,
            @event.CardNumber, @event.CardHolderName, @event.CardExpiration,
            @event.CardSecurityNumber, @event.CardTypeId, "");

        await _mediator.Send(createOrderCommand);
    }
}