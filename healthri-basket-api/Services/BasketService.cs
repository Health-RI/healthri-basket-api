using healthri_basket_api.Helpers;
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

    public async Task<Basket?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct)
    {
        var basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.UserId != userId)
            return null;
        
        return basket;
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
        while (await basketRepository.GetBySlugAsync(userId, slug, ct) != null)
        {
            slug = $"{originalSlug}-{counter}";
            counter++;
        }
        
        Basket basket = new Basket(userId, slug, name, isDefault);
        
        await basketRepository.CreateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.CreateBasket, BasketItemSource.UserPage);
        return basket;
    }

    public async Task<bool> RenameAsync(Guid userId, string slug, string newName, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.UserId != userId) return false;

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
        Basket? existingBasket = await basketRepository.GetBySlugAsync(userId, newSlug, ct);
        while (existingBasket != null && existingBasket.Id != basket.Id)
        {
            newSlug = $"{originalNewSlug}-{counter}";
            counter++;
            existingBasket = await basketRepository.GetBySlugAsync(userId, newSlug, ct);
        }

        basket.Rename(newName, newSlug);
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RenameBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.UserId != userId || basket.IsDefault) return false;

        basket.Delete();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.DeleteBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> RestoreAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.UserId != userId) return false;

        basket.Restore();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.RestoreBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ArchiveAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.UserId != userId) return false;

        basket.Archive();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.ArchiveBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<bool> ClearAsync(Guid userId, string slug, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.UserId != userId) return false;

        basket.ClearItems();
        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, Guid.Empty, BasketAction.ClearBasket, BasketItemSource.UserPage);

        return true;
    }

    public async Task<Basket?> AddItemAsync(Guid userId, string slug, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        try
        {
            Basket? basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
            if (basket == null)
                throw new InvalidOperationException("Basket not found");
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

    public async Task<bool> RemoveItemAsync(Guid userId, string slug, Guid itemId, BasketItemSource source, CancellationToken ct)
    {
        Basket? basket = await basketRepository.GetBySlugAsync(userId, slug, ct);
        if (basket == null || basket.UserId != userId) return false;

        // Update in-memory basket model
        basket.RemoveItem(itemId);

        await basketRepository.UpdateAsync(basket, ct);
        await logger.LogAsync(basket.UserId, basket.Id, itemId, BasketAction.RemoveItem, source);

        return true;
    }
}