using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Application.IntegrationEvents;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Features.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var address = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);
        Order order = new Order(request.UserName, address, request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration);

        if (request.OrderItems != null)
        {
            foreach (var orderItem in request.OrderItems)
                order.AddOrderItem(orderItem.ProductId, orderItem.ProductName, orderItem.UnitPrice, orderItem.PictureUrl, orderItem.Units);
        }

        await _orderRepository.AddAsync(order);
        await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(request.UserName);

        _eventBus.Publish(orderStartedIntegrationEvent);
            
        return true;
    }
}