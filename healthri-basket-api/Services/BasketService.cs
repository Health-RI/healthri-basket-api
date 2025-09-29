using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;
using healthri_basket_api.Repositories;

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

    public async Task<IEnumerable<Basket>> GetBasketsAsync(Guid userUuid)
    {
        return await _basketRepository.GetUserBasketsAsync(userUuid);
    }

    public async Task<Basket?> GetByIdAsync(Guid basketId)
    {
        return await _basketRepository.GetBasketByIdAsync(basketId);
    }

    public async Task<Basket> CreateBasketAsync(Guid userId, string name, bool isDefault)
    {
        var basket = new Basket(userId, name, isDefault);
        
        await _basketRepository.CreateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.CreateBasket, BasketItemSource.UserPage);
        return basket;
    }

    public async Task<bool> RenameBasketAsync(Guid basketId, string newName)
    {
        var basket = await _basketRepository.GetBasketByIdAsync(basketId);
        if (basket == null) return false;

        basket.Rename(newName);
        await _basketRepository.UpdateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> DeleteBasketAsync(Guid basketId)
    {
        var basket = await _basketRepository.GetBasketByIdAsync(basketId);
        if (basket == null || basket.IsDefault) return false;

        basket.Delete();
        await _basketRepository.UpdateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> RestoreBasketAsync(Guid basketId)
    {
        var basket = await _basketRepository.GetBasketByIdAsync(basketId);
        if (basket == null) return false;

        basket.Restore();
        await _basketRepository.UpdateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ArchiveBasketAsync(Guid basketId)
    {
        var basket = await _basketRepository.GetBasketByIdAsync(basketId);
        if (basket == null) return false;

        basket.Archive();
        await _basketRepository.UpdateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ClearBasketAsync(Guid basketId)
    {
        var basket = await _basketRepository.GetBasketByIdAsync(basketId);
        if (basket == null) return false;

        basket.ClearItems();
        await _basketRepository.UpdateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> AddItemToBasketAsync(Guid basketId, Guid itemId, BasketItemSource source)
    {
        var basket = await _basketRepository.GetBasketByIdAsync(basketId);
        if (basket == null) return false;

        var item = await _itemService.GetItemByIdAsync(itemId);
        if (item == null) return false;

        var basketItem = new BasketItem(basket, item);
        await _basketRepository.AddItemToBasketAsync(basketItem);

        await _basketRepository.UpdateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, itemId, BasketAction.RenameBasket, source);

        return true;
    }

    public async Task<bool> RemoveItemFromBasketAsync(Guid basketId, Guid itemId, BasketItemSource source)
    {
        var basket = await _basketRepository.GetBasketByIdAsync(basketId);
        if (basket == null) return false;

        var item = await _itemService.GetItemByIdAsync(itemId);
        if (item == null) return false;
        basket.RemoveItem(item.Id);

        await _basketRepository.UpdateBasketAsync(basket);
        await _logger.LogAsync(basket.UserId, basket.Id, itemId, BasketAction.RemoveItem, source);

        return true;
    }
}