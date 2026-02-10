using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Repositories;

public class BasketRepository(AppDbContext context) : IBasketRepository
{
    public async Task<Basket> CreateAsync(Basket basket, CancellationToken ct)
    {
        await context.Baskets.AddAsync(basket, ct);
        await context.SaveChangesAsync(ct);
        return basket;
    }

    public async Task<Basket> UpdateAsync(Basket basket, CancellationToken ct)
    {
        context.Baskets.Update(basket);
        await context.SaveChangesAsync(ct);
        return basket;
    }

    public async Task<bool> DeleteAsync(Basket basket, CancellationToken ct)
    {
        context.Baskets.Remove(basket);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AddItemAsync(BasketItem basketItem, CancellationToken ct)
    {
        await context.BasketItems.AddAsync(basketItem, ct);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveItemAsync(BasketItem basketItem, CancellationToken ct)
    {
        context.BasketItems.Remove(basketItem);
        await context.SaveChangesAsync(ct);
        return true;
    }


    public async Task<List<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await context.Baskets
            .Include(b => b.Items)
            .Where(b => b.UserId.Equals(userId))
            .ToListAsync(ct);
    }

    public async Task<Basket?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id.Equals(id), ct);
    }

}
