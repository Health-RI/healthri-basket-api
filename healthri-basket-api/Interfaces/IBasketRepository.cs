using healthri_basket_api.Models;

namespace healthri_basket_api.Interfaces;

public interface IBasketRepository
{
    Task<List<Basket>> GetUserBasketsAsync(Guid userUuid);
    Task<Basket?> GetBasketByIdAsync(Guid id);
    Task AddItemToBasketAsync(BasketItem basketItem);
    Task CreateBasketAsync(Basket basket);
    Task UpdateBasketAsync(Basket basket);
    Task DeleteBasketAsync(Basket basket);
}
