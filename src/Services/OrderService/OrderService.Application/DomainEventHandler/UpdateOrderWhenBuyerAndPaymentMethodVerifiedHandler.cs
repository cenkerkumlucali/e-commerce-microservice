using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Events;

namespace OrderService.Application.DomainEventHandler;

public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedHandler : INotificationHandler<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderWhenBuyerAndPaymentMethodVerifiedHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(BuyerAndPaymentMethodVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetByIdAsync(notification.OrderId);
        orderToUpdate.SetBuyerId(notification.Buyer.Id);
        orderToUpdate.SetPaymentMethodId(notification.Payment.Id);

        //set methods so validate
    }
}