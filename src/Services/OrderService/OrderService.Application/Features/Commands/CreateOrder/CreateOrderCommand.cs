using MediatR;
using OrderService.Domain.Models;

namespace OrderService.Application.Features.Commands.CreateOrder;

 public class CreateOrderCommand : IRequest<bool>
    {
        public readonly List<OrderItemDto> _orderItems;
        public string UserName { get; private set; }
        public string City { get; private set; }
        public string Street { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }
        public string CardNumber { get; private set; }
        public string CardHolderName { get; private set; }
        public DateTime CardExpiration { get; private set; }
        public string CardSecurityNumber { get; private set; }
        public int CardTypeId { get; private set; }

        public IEnumerable<OrderItemDto> OrderItems => _orderItems;

        public string CorrelationId { get; set; }

        public CreateOrderCommand()
        {
            _orderItems = new List<OrderItemDto>();
        }

        public CreateOrderCommand(List<BasketItem> basketItems, string userName, string city, string street, string state, string country, string zipCode, string cardNumber, string cardHolderName, DateTime cardExpiration, string cardSecurityNumber, int cardTypeId, string correlationId)
        {
            _orderItems = basketItems.Select(item => new OrderItemDto()
            {
                ProductId = item.ProductId,
                PictureUrl = item.PictureUrl,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Units = item.Quantity
            })?.ToList();

            UserName = userName;
            City = city;
            Street = street;
            State = state;
            Country = country;
            ZipCode = zipCode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            CorrelationId = correlationId;
        }
    }

    public class OrderItemDto
    {
        public int ProductId { get; init; }
        public string ProductName { get; init; }
        public decimal UnitPrice { get; init; }
        public string PictureUrl { get; init; }
        public int Units { get; init; }
    }