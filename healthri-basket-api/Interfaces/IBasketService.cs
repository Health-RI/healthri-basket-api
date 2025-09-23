using healthri_basket_api.Models;

namespace healthri_basket_api.Interfaces;

public interface IBasketService
{
    Task<IEnumerable<Basket>> GetBasketsAsync(Guid userUuid);
    Task<Basket?> GetByIdAsync(Guid basketId);
    Task<Basket> CreateBasketAsync(Guid userUuid, string name, bool isDefault = false);
    Task<bool> RenameBasketAsync(Guid basketId, string newName);
    Task<bool> DeleteBasketAsync(Guid basketId);
    Task<bool> RestoreBasketAsync(Guid basketId);
    Task<bool> ArchiveBasketAsync(Guid basketId);
    Task<bool> ClearBasketAsync(Guid basketId);
    Task<bool> AddItemAsync(Guid basketId, Guid itemId, string source);
    Task<bool> RemoveItemAsync(Guid basketId, Guid itemId);
}