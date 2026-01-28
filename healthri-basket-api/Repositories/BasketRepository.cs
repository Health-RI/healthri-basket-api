using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly AppDbContext _context;

    public BasketRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Basket> CreateAsync(Basket basket, CancellationToken ct)
    {
        await _context.Baskets.AddAsync(basket, ct);
        await _context.SaveChangesAsync(ct);
        return basket;
    }

    public async Task<Basket> UpdateAsync(Basket basket, CancellationToken ct)
    {
        _context.Baskets.Update(basket);
        await _context.SaveChangesAsync(ct);
        return basket;
    }

    public async Task<bool> DeleteAsync(Basket basket, CancellationToken ct)
    {
        _context.Baskets.Remove(basket);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> AddItemAsync(BasketItem item, CancellationToken ct)
    {
        await _context.BasketItems.AddAsync(item, ct);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveItemAsync(BasketItem item, CancellationToken ct)
    {
        _context.BasketItems.Remove(item);
        await _context.SaveChangesAsync(ct);
        return true;
    }


    public async Task<List<Basket>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        return await _context.Baskets
            .Include(b => b.Items)
            .Where(b => b.UserId.Equals(userId))
            .ToListAsync(ct);
    }

    public async Task<Basket?> GetByIdAsync(Guid basketId, CancellationToken ct)
    {
        return await _context.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.Id.Equals(basketId), ct);
    }

    public async Task<Basket?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct)
    {
        return await _context.Baskets
            .Include(b => b.Items)
                .ThenInclude(bi => bi.Item)
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Slug == slug, ct);
    }

}