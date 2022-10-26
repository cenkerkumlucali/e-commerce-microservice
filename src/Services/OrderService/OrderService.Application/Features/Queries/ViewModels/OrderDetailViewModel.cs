namespace OrderService.Application.Features.Queries.ViewModels;

public class OrderDetailViewModel
{
    public string OrderNumber { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string Zipcode { get; set; }
    public string Country { get; set; }
    public List<OrderItem> OrderItems { get; set; }
    public decimal Total { get; set; }
}

public class OrderItem
{
    public string ProductName { get; set; }
    public int Units { get; set; }
    public double UnitPrice { get; set; }
    public string PictureUrl { get; set; }
}