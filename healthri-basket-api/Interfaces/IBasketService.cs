using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

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
    Task<bool> AddItemToBasketAsync(Guid basketId, Guid itemId, BasketItemSource source);
    Task<bool> RemoveItemFromBasketAsync(Guid basketId, Guid itemId, BasketItemSource source);
}