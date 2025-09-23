using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;

namespace healthri_basket_api.Services;

public class BasketService(IBasketRepository repository, ITransactionLogger logger) : IBasketService
{
    public async Task<IEnumerable<Basket>> GetBasketsAsync(Guid userUuid)
    {
        return await repository.GetUserBasketsAsync(userUuid);
    }

    public async Task<Basket?> GetByIdAsync(Guid basketId)
    {
        return await repository.GetByIdAsync(basketId);
    }

    public async Task<Basket> CreateBasketAsync(Guid userUuid, string name, bool isDefault = false)
    {
        var basket = new Basket
        {
            UserUuid = userUuid,
            Name = name,
            IsDefault = isDefault
        };

        await repository.AddAsync(basket);
        await logger.LogAsync(userUuid, basket.Id, string.Empty, "create_basket", "api");

        return basket;
    }

    public async Task<bool> RenameBasketAsync(Guid basketId, string newName)
    {
        var basket = await repository.GetByIdAsync(basketId);
        if (basket == null) return false;

        basket.Rename(newName);
        await repository.UpdateAsync(basket);
        await logger.LogAsync(basket.UserUuid, basket.Id, string.Empty, "rename_basket", "api");

        return true;
    }

    public async Task<bool> DeleteBasketAsync(Guid basketId)
    {
        var basket = await repository.GetByIdAsync(basketId);
        if (basket == null || basket.IsDefault) return false;

        basket.Delete();
        await repository.UpdateAsync(basket);
        await logger.LogAsync(basket.UserUuid, basket.Id, string.Empty, "delete_basket", "api");

        return true;
    }

    public async Task<bool> RestoreBasketAsync(Guid basketId)
    {
        var basket = await repository.GetByIdAsync(basketId);
        if (basket == null) return false;

        basket.Restore();
        await repository.UpdateAsync(basket);
        await logger.LogAsync(basket.UserUuid, basket.Id, string.Empty, "restore_basket", "api");

        return true;
    }

    public async Task<bool> ArchiveBasketAsync(Guid basketId)
    {
        var basket = await repository.GetByIdAsync(basketId);
        if (basket == null) return false;

        basket.Archive();
        await repository.UpdateAsync(basket);
        await logger.LogAsync(basket.UserUuid, basket.Id, string.Empty, "archive_basket", "api");

        return true;
    }

    public async Task<bool> ClearBasketAsync(Guid basketId)
    {
        var basket = await repository.GetByIdAsync(basketId);
        if (basket == null) return false;

        basket.ClearItems();
        await repository.UpdateAsync(basket);
        await logger.LogAsync(basket.UserUuid, basket.Id, string.Empty, "clear_basket", "api");

        return true;
    }

    public async Task<bool> AddItemAsync(Guid basketId, Guid itemId, string source)
    {
        var basket = await repository.GetByIdAsync(basketId);
        if (basket == null) return false;

        basket.AddItem(new BasketItem
        {
            ItemId = itemId,
            Source = source
        });

        await repository.UpdateAsync(basket);
        await logger.LogAsync(basket.UserUuid, basket.Id, itemId.ToString(), "add", source);

        return true;
    }

    public async Task<bool> RemoveItemAsync(Guid basketId, Guid itemId)
    {
        var basket = await repository.GetByIdAsync(basketId);
        if (basket == null) return false;

        basket.RemoveItem(itemId);
        await repository.UpdateAsync(basket);
        await logger.LogAsync(basket.UserUuid, basket.Id, itemId.ToString(), "remove", "api");

        return true;
    }
}