using MediatR;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Domain.Events;

public class OrderStartedDomainEvent: INotification
{
    public string UserName { get; }

    public int CardTypeId { get; }
    public string CardNumber { get; }
    public string CardSecurityNumber { get; }
    public string CardHolderName { get; }
    public DateTime CardExpiration { get; }
    public Order Order { get; }

    public OrderStartedDomainEvent(string userName, int cardTypeId, string cardNumber, string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
    {
        UserName = userName;
        CardTypeId = cardTypeId;
        CardNumber = cardNumber;
        CardSecurityNumber = cardSecurityNumber;
        CardHolderName = cardHolderName;
        CardExpiration = cardExpiration;
    }
}