using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;

namespace healthri_basket_api.Services;

public class ItemService : IItemService
{
    private readonly IItemRepository _itemRepository;

    public ItemService(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<Item?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var item = await _itemRepository.GetByIdAsync(id, ct);
        return item;
    }
}
