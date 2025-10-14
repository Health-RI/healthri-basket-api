using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Interfaces;

public interface IBasketService
{
    Task<IEnumerable<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Basket?> GetByIdAsync(Guid basketId, CancellationToken ct);
    Task<Basket> CreateAsync(Guid userId, string name, bool isDefault, CancellationToken ct);
    Task<bool> RenameAsync(Guid basketId, string newName, CancellationToken ct);
    Task<bool> DeleteAsync(Guid basketId, CancellationToken ct);
    Task<bool> RestoreAsync(Guid basketId, CancellationToken ct);
    Task<bool> ArchiveAsync(Guid basketId, CancellationToken ct);
    Task<bool> ClearAsync(Guid basketId, CancellationToken ct);
    Task<Basket> AddItemAsync(Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct);
    Task<bool> RemoveItemAsync(Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct);
}