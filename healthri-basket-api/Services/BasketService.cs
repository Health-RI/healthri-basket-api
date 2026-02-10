using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Services;

public class BasketService(IBasketRepository basketRepository, IItemService itemService, ITransactionLogger logger)
    : IBasketService
{
    public async Task<IEnumerable<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await basketRepository.GetByUserIdAsync(userId, ct);
    }

    public async Task<Basket?> GetByIdAsync(Guid userId, Guid basketId, CancellationToken ct)
    {
        var basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.UserId != userId)
            return null;
        
        return basket;
    }

    public async Task<Basket> CreateAsync(Guid userId, string name, bool isDefault, CancellationToken ct)
    {
        Basket basket = new Basket(userId, name, isDefault);
        
        await basketRepository.CreateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.CreateBasket, BasketItemSource.UserPage);
        return basket;
    }

    public async Task<bool> RenameAsync(Guid userId, Guid basketId, string newName, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.UserId != userId) return false;

        basket.Rename(newName);
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid basketId, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.UserId != userId || basket.IsDefault) return false;

        basket.Delete();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> RestoreAsync(Guid userId, Guid basketId, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.UserId != userId) return false;

        basket.Restore();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ArchiveAsync(Guid userId, Guid basketId, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.UserId != userId) return false;

        basket.Archive();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ClearAsync(Guid userId, Guid basketId, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.UserId != userId) return false;

        basket.ClearItems();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<Basket?> AddItemAsync(Guid userId, Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        try
        {
            Basket basket = await basketRepository.GetByIdAsync(basketId, ct)
                         ?? throw new InvalidOperationException("Basket not found");

            if (basket.UserId != userId)
                return null;

            if (basket.HasItem(itemId))
                throw new InvalidOperationException("Item already in basket");

            // Retrieve item
            Item item = await itemService.GetByIdAsync(itemId, ct)
                       ?? throw new InvalidOperationException("Item not found");

            BasketItem basketItem = new BasketItem(basket, item);

            await basketRepository.AddItemAsync(basketItem, ct);

            basket.AddItem(item); // Update in-memory

            await logger.LogAsync(basket.UserId, basket.Id, item.Id, BasketAction.AddItem, source);

            return basket;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding item to basket: " + ex.Message);
            return null;
        }
    }


    public async Task<bool> RemoveItemAsync(Guid userId, Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.UserId != userId) return false;

        // Update in-memory basket model
        basket.RemoveItem(itemId);

        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, itemId, BasketAction.RemoveItem, source);

        basket = await basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null) return false;
        return true;
    }
}