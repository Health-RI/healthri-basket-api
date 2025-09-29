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

    public async Task<List<Basket>> GetUserBasketsAsync(Guid userId)
    {
        return await _context.Baskets
            .Where(b => b.UserId == userId)
            .ToListAsync();
    }

    public async Task<Basket?> GetBasketByIdAsync(Guid basketId)
    {
        return await _context.Baskets.FirstOrDefaultAsync(b => b.Id == basketId);
    }

    public async Task CreateBasketAsync(Basket basket)
    {
        await _context.Baskets.AddAsync(basket);
        await _context.SaveChangesAsync();
    }

    public async Task AddItemToBasketAsync(BasketItem basketItem)
    {
        await _context.BasketItems.AddAsync(basketItem);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateBasketAsync(Basket basket)
    {
        _context.Baskets.Update(basket);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteBasketAsync(Basket basket)
    {
        _context.Baskets.Remove(basket);
        await _context.SaveChangesAsync();
    }
}