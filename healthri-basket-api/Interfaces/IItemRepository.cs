using healthri_basket_api.Models;

namespace healthri_basket_api.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id, CancellationToken ct);   
}