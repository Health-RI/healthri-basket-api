using healthri_basket_api.Models;

namespace healthri_basket_api.Interfaces;

public interface IBasketRepository
{
    Task<IEnumerable<Basket>> GetUserBasketsAsync(Guid userUuid);
    Task<Basket?> GetByIdAsync(Guid id);
    Task AddAsync(Basket basket);
    Task UpdateAsync(Basket basket);
    Task DeleteAsync(Guid id);
}
