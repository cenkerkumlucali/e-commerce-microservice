using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Events;

namespace OrderService.Application.DomainEventHandler;

public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
{
    private readonly IBuyerRepository _buyerRepository;

    public OrderStartedDomainEventHandler(IBuyerRepository buyerRepository)
    {
        _buyerRepository = buyerRepository;
    }

    public async Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
    {
        var cardTypeId = (notification.CardTypeId != 0) ? notification.CardTypeId : 1;

        var buyer = await _buyerRepository.GetSingleAsync(i => i.Name == notification.UserName, i => i.PaymentMethods);

        bool buyerOriginallyExisted = buyer != null;

        if (!buyerOriginallyExisted)
        {
            buyer = new Domain.AggregateModels.BuyerAggregate.Buyer(notification.UserName);
        }

        buyer.VerifyOrAddPaymentMethod(cardTypeId,
            $"Payment Method on {DateTime.UtcNow}",
            notification.CardNumber,
            notification.CardSecurityNumber,
            notification.CardHolderName,
            notification.CardExpiration,
            notification.Order.Id);

        var buyerUpdated = buyerOriginallyExisted ?
            _buyerRepository.Update(buyer) :
            await _buyerRepository.AddAsync(buyer);

        await _buyerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        // order status changed event may be fired here
    }
}