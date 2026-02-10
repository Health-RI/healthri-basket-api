using healthri_basket_api.Database;
using healthri_basket_api.Models;
using healthri_basket_api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.test.Repositories.Tests;

public class BasketRepositoryTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_AddsBasket()
    {
        using var context = CreateContext();
        var repo = new BasketRepository(context);
        var basket = new Basket(Guid.NewGuid(), "basket", "Basket", false);

        await repo.CreateAsync(basket, CancellationToken.None);

        var saved = await context.Baskets.FirstOrDefaultAsync(b => b.Id == basket.Id);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task UpdateAsync_WhenCalled_UpdatesBasket()
    {
        using var context = CreateContext();
        var repo = new BasketRepository(context);
        var basket = new Basket(Guid.NewGuid(), "basket", "Basket", false);
        context.Baskets.Add(basket);
        await context.SaveChangesAsync();

        basket.Name = "Updated";
        await repo.UpdateAsync(basket, CancellationToken.None);

        var updated = await context.Baskets.AsNoTracking().FirstOrDefaultAsync(b => b.Id == basket.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Name);
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_RemovesBasket()
    {
        using var context = CreateContext();
        var repo = new BasketRepository(context);
        var basket = new Basket(Guid.NewGuid(), "basket", "Basket", false);
        context.Baskets.Add(basket);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(basket, CancellationToken.None);

        var exists = await context.Baskets.AnyAsync(b => b.Id == basket.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task AddItemAsync_WhenCalled_AddsBasketItem()
    {
        using var context = CreateContext();
        var repo = new BasketRepository(context);
        var basket = new Basket(Guid.NewGuid(), "basket", "Basket", false);
        var item = new Item("Item", "Desc");
        context.Baskets.Add(basket);
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var basketItem = new BasketItem(basket, item);
        await repo.AddItemAsync(basketItem, CancellationToken.None);

        var exists = await context.BasketItems.AnyAsync(bi => bi.BasketId == basket.Id && bi.ItemId == item.Id);
        Assert.True(exists);
    }

    [Fact]
    public async Task RemoveItemAsync_WhenCalled_RemovesBasketItem()
    {
        using var context = CreateContext();
        var repo = new BasketRepository(context);
        var basket = new Basket(Guid.NewGuid(), "basket", "Basket", false);
        var item = new Item("Item", "Desc");
        var basketItem = new BasketItem(basket, item);
        context.Baskets.Add(basket);
        context.Items.Add(item);
        context.BasketItems.Add(basketItem);
        await context.SaveChangesAsync();

        await repo.RemoveItemAsync(basketItem, CancellationToken.None);

        var exists = await context.BasketItems.AnyAsync(bi => bi.Id == basketItem.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenCalled_ReturnsBasketsForUser()
    {
        using var context = CreateContext();
        var repo = new BasketRepository(context);
        Guid userId = Guid.NewGuid();
        var basket1 = new Basket(userId, "basket-1", "Basket 1", false);
        var basket2 = new Basket(userId, "basket-2", "Basket 2", false);
        var otherBasket = new Basket(Guid.NewGuid(), "other", "Other", false);
        context.Baskets.AddRange(basket1, basket2, otherBasket);
        await context.SaveChangesAsync();

        var result = await repo.GetByUserIdAsync(userId, CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.All(result, b => Assert.Equal(userId, b.UserId));
    }

    [Fact]
    public async Task GetBySlugAsync_WhenCalled_ReturnsBasket()
    {
        using var context = CreateContext();
        var repo = new BasketRepository(context);
        var basket = new Basket(Guid.NewGuid(), "basket", "Basket", false);
        context.Baskets.Add(basket);
        await context.SaveChangesAsync();

        var result = await repo.GetBySlugAsync(basket.UserId, basket.Slug, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(basket.Id, result.Id);
    }
}
