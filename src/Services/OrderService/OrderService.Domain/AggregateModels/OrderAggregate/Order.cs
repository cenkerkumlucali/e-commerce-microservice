using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;

namespace OrderService.Domain.AggregateModels.OrderAggregate;

public class Order: BaseEntity, IAggregateRoot
    {
        public DateTime OrderDate { get; private set; }
        public int Quantity { get; private set; }
        public string Description { get; private set; }
        public Guid? BuyerId { get; private set; }
        public Buyer Buyer { get; private set; }
        public Address Address { get; private set; }

        private int orderStatusId;
        public OrderStatus OrderStatus { get; private set; }

        private readonly List<OrderItem> _orderItems;

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public Guid? PaymentMethodId { get; set; }

        protected Order()
        {
            Id = Guid.NewGuid();
            _orderItems = new List<OrderItem>();
        }

        public Order(string userName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber,
          string cardHolderName, DateTime cardExpiration, Guid? buyerId = null, Guid? paymentMethodId = null) : this()
        {
            BuyerId = buyerId;
            PaymentMethodId = paymentMethodId;
            orderStatusId = OrderStatus.Submitted.Id;
            OrderDate = DateTime.UtcNow;
            Address = address;

            // Add the OrderStarterDomainEvent to the domain events collection 
            // to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
            AddOrderStartedDomainEvent(userName, cardTypeId, cardNumber,
                                        cardSecurityNumber, cardHolderName, cardExpiration);
        }

        private void AddOrderStartedDomainEvent(string userName, int cardTypeId, string cardNumber,
         string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(userName, cardTypeId, cardNumber,
                                                                      cardSecurityNumber, cardHolderName, cardExpiration);

            this.AddDomainEvent(orderStartedDomainEvent);
        }

        // DDD Patterns comment
        // This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
        // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
        // in order to maintain consistency between the whole Aggregate. 
        public void AddOrderItem(int productId, string productName, decimal unitPrice, string pictureUrl, int units = 1)
        {
            //add validated new order item

            var orderItem = new OrderItem(productId, productName, pictureUrl, unitPrice, units = 1);
            _orderItems.Add(orderItem);
        }

        public void SetBuyerId(Guid id)
        {
            BuyerId = id;
        }

        public void SetPaymentMethodId(Guid paymentMethodId)
        {
            PaymentMethodId = paymentMethodId;
        }

    }