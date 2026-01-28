using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Interfaces;

public interface IBasketService
{
    Task<IEnumerable<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Basket?> GetByIdAsync(Guid userId, Guid basketId, CancellationToken ct);
    Task<Basket> CreateAsync(Guid userId, string name, bool isDefault, CancellationToken ct);
    Task<bool> RenameAsync(Guid userId, Guid basketId, string newName, CancellationToken ct);
    Task<bool> DeleteAsync(Guid userId, Guid basketId, CancellationToken ct);
    Task<bool> RestoreAsync(Guid userId, Guid basketId, CancellationToken ct);
    Task<bool> ArchiveAsync(Guid userId, Guid basketId, CancellationToken ct);
    Task<bool> ClearAsync(Guid userId, Guid basketId, CancellationToken ct);
    Task<Basket?> AddItemAsync(Guid userId, Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct);
    Task<bool> RemoveItemAsync(Guid userId, Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct);
}