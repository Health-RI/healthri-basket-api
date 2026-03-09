using healthri_basket_api.Models;

namespace healthri_basket_api.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(string id, CancellationToken ct);   
}
