using System.ComponentModel.DataAnnotations;
using OrderService.Domain.SeedWork;

namespace OrderService.Domain.AggregateModels.OrderAggregate;

public class OrderItem: BaseEntity, IValidatableObject
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string PictureUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }

    protected OrderItem()
    {
    }

    public OrderItem(int productId, string productName, string pictureUrl, decimal unitPrice, int units)
    {
        ProductId = productId;
        ProductName = productName;
        PictureUrl = pictureUrl;
        UnitPrice = unitPrice;
        Units = units;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (Units <= 0)
            results.Add(new("Invalid number of units", new[] { "Units" }));

        return results;
    }
}