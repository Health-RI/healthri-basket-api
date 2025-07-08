using healthri_basket_api.Models;
using healthri_basket_api.Interfaces;

namespace healthri_basket_api.Repositories;

public class InMemoryBasketRepository : IBasketRepository
{
    private readonly Dictionary<Guid, Basket> _baskets = new();

    public Task<IEnumerable<Basket>> GetUserBasketsAsync(Guid userUuid)
    {
        var results = _baskets.Values.Where(b => b.UserUuid == userUuid);
        return Task.FromResult(results);
    }

    public Task<Basket?> GetByIdAsync(Guid id)
    {
        _baskets.TryGetValue(id, out var basket);
        return Task.FromResult(basket);
    }

    public Task AddAsync(Basket basket)
    {
        _baskets[basket.Id] = basket;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Basket basket)
    {
        _baskets[basket.Id] = basket;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _baskets.Remove(id);
        return Task.CompletedTask;
    }
}