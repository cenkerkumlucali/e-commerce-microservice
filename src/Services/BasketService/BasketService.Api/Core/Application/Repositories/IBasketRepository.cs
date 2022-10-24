using BasketService.Api.Core.Domain.Models;

namespace BasketService.Api.Core.Application.Repositories;

public interface IBasketRepository
{
    Task<CustomerBasket> GetBasketAsync(string customerId);

    IEnumerable<string> GetUsers();

    Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket);

    Task<bool> DeleteBasketAsync(string id);
}