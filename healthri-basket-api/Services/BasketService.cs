using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Services;

public class BasketService : IBasketService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IItemService _itemService;
    private readonly ITransactionLogger _logger;

    public BasketService(IBasketRepository basketRepository, IItemService itemService, ITransactionLogger logger)
    {
        _basketRepository = basketRepository;
        _itemService = itemService;
        _logger = logger;
    }

    public async Task<IEnumerable<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _basketRepository.GetByUserIdAsync(userId, ct);
    }

    public async Task<Basket?> GetByIdAsync(Guid basketId, CancellationToken ct)
    {
        return await _basketRepository.GetByIdAsync(basketId, ct);
    }

    public async Task<Basket> CreateAsync(Guid userId, string name, bool isDefault, CancellationToken ct)
    {
        Basket basket = new Basket(userId, name, isDefault);
        
        await _basketRepository.CreateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.CreateBasket, BasketItemSource.UserPage);
        return basket;
    }

    public async Task<bool> RenameAsync(Guid basketId, string newName, CancellationToken ct)
    {
        Basket basket = await _basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null) return false;

        basket.Rename(newName);
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid basketId, CancellationToken ct)
    {
        Basket basket = await _basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null || basket.IsDefault) return false;

        basket.Delete();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> RestoreAsync(Guid basketId, CancellationToken ct)
    {
        Basket basket = await _basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null) return false;

        basket.Restore();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ArchiveAsync(Guid basketId, CancellationToken ct)
    {
        Basket basket = await _basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null) return false;

        basket.Archive();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ClearAsync(Guid basketId, CancellationToken ct)
    {
        Basket basket = await _basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null) return false;

        basket.ClearItems();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<Basket?> AddItemAsync(Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        try
        {
            Basket basket = await _basketRepository.GetByIdAsync(basketId, ct)
                         ?? throw new InvalidOperationException("Basket not found");

            if (basket.HasItem(itemId))
                throw new InvalidOperationException("Item already in basket");

            // Retrieve item
            Item item = await _itemService.GetByIdAsync(itemId, ct)
                       ?? throw new InvalidOperationException("Item not found");

            BasketItem basketItem = new BasketItem(basket, item);

            await _basketRepository.AddItemAsync(basketItem, ct);

            basket.AddItem(item); // Update in-memory

            await _logger.LogAsync(basket.UserId, basket.Id, item.Id, BasketAction.AddItem, source);

            return basket;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error adding item to basket: " + ex.Message);
            return null;
        }
    }


    public async Task<bool> RemoveItemAsync(Guid basketId, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        Basket basket = await _basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null) return false;

        // Update in-memory basket model
        basket.RemoveItem(itemId);

        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, itemId, BasketAction.RemoveItem, source);

        basket = await _basketRepository.GetByIdAsync(basketId, ct);
        if (basket == null) return false;
        return true;
    }
}