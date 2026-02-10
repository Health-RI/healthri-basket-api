using healthri_basket_api.Models;

namespace healthri_basket_api.Interfaces;

public interface IBasketRepository
{
    Task<List<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Basket?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> AddItemAsync(BasketItem basketItem, CancellationToken ct);
    Task<bool> RemoveItemAsync(BasketItem basketItem, CancellationToken ct);
    Task<Basket> CreateAsync(Basket basket, CancellationToken ct);
    Task<Basket> UpdateAsync(Basket basket, CancellationToken ct);
    Task<bool> DeleteAsync(Basket basket, CancellationToken ct);
}
