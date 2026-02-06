using healthri_basket_api.Helpers;
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

    public async Task<Basket?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct)
    {
        return await _basketRepository.GetBySlugAsync(userId, slug, ct);
    }

    public async Task<Basket> CreateAsync(Guid userId, string name, bool isDefault, string? customSlug, CancellationToken ct)
    {
        string slug;
        
        if (!string.IsNullOrWhiteSpace(customSlug))
        {
            // Validate custom slug is properly formatted
            string slugified = SlugHelper.Slugify(customSlug);
            if (slugified != customSlug)
            {
                throw new ArgumentException($"Custom slug '{customSlug}' is not valid. Use only lowercase letters, numbers, and hyphens.", nameof(customSlug));
            }
            slug = customSlug;
        }
        else
        {
            // Generate slug from name
            slug = SlugHelper.Slugify(name);
            
            if (string.IsNullOrEmpty(slug))
            {
                throw new ArgumentException("Basket name must contain at least one valid character for slug generation.", nameof(name));
            }
        }
        
        // Ensure slug uniqueness for this user
        int counter = 1;
        string originalSlug = slug;
        while (await _basketRepository.GetBySlugAsync(userId, slug, ct) != null)
        {
            slug = $"{originalSlug}-{counter}";
            counter++;
        }
        
        Basket basket = new Basket(userId, slug, name, isDefault);
        
        await _basketRepository.CreateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.CreateBasket, BasketItemSource.UserPage);
        return basket;
    }

    public async Task<bool> RenameAsync(Guid userId, string slug, string newName, CancellationToken ct)
    {
        Basket? basket = await _basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null) return false;

        // Validate newName and ensure it generates a valid slug
        if (string.IsNullOrWhiteSpace(newName))
        {
            return false;
        }

        string newSlug = SlugHelper.Slugify(newName);
        
        // If slug generation fails, keep the existing name and slug
        if (string.IsNullOrEmpty(newSlug))
        {
            return false;
        }
        
        // Ensure new slug uniqueness (skip if it's the same basket)
        int counter = 1;
        string originalNewSlug = newSlug;
        Basket? existingBasket = await _basketRepository.GetBySlugAsync(userId, newSlug, ct);
        while (existingBasket != null && existingBasket.Id != basket.Id)
        {
            newSlug = $"{originalNewSlug}-{counter}";
            counter++;
            existingBasket = await _basketRepository.GetBySlugAsync(userId, newSlug, ct);
        }

        basket.Rename(newName, newSlug);
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await _basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.IsDefault) return false;

        basket.Delete();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.DeleteBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> RestoreAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await _basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null) return false;

        basket.Restore();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RestoreBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ArchiveAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await _basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null) return false;

        basket.Archive();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.ArchiveBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ClearAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await _basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null) return false;

        basket.ClearItems();
        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.ClearBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<Basket?> AddItemAsync(Guid userId, string slug, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        try
        {
            Basket? basket = await _basketRepository.GetBySlugAsync(userId, slug, ct);
            if (basket == null)
                throw new InvalidOperationException("Basket not found");

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


    public async Task<bool> RemoveItemAsync(Guid userId, string slug, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        Basket? basket = await _basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null) return false;

        // Update in-memory basket model
        basket.RemoveItem(itemId);

        await _basketRepository.UpdateAsync(basket, ct);
        await _logger.LogAsync(basket.UserId, basket.Id, itemId, BasketAction.RemoveItem, source);

        return true;
    }
}