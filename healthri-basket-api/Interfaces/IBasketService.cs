using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Interfaces;

public interface IBasketService
{
    Task<IEnumerable<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<Basket?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct);
    Task<Basket> CreateAsync(Guid userId, string name, bool isDefault, string? customSlug, CancellationToken ct);
    Task<bool> RenameAsync(Guid userId, string slug, string newName, CancellationToken ct);
    Task<bool> DeleteAsync(Guid userId, string slug, CancellationToken ct);
    Task<bool> RestoreAsync(Guid userId, string slug, CancellationToken ct);
    Task<bool> ArchiveAsync(Guid userId, string slug, CancellationToken ct);
    Task<bool> ClearAsync(Guid userId, string slug, CancellationToken ct);
    Task<Basket?> AddItemAsync(Guid userId, string slug, Guid itemId, BasketItemSource source, CancellationToken ct);
    Task<bool> RemoveItemAsync(Guid userId, string slug, Guid itemId, BasketItemSource source, CancellationToken ct);
}