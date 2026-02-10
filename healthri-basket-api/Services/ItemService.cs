using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;

namespace healthri_basket_api.Services;

public class ItemService(IItemRepository itemRepository) : IItemService
{
    public async Task<Item?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var item = await itemRepository.GetByIdAsync(id, ct);
        return item;
    }
}
