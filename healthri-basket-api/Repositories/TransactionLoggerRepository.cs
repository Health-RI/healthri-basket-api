using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Repositories;

public class TransactionLoggerRepository(AppDbContext context) : ITransactionLogger
{
    public Task LogAsync(Guid userId, Guid basketId, Guid itemId, BasketAction action, BasketItemSource source)
    {
        context.Add(new TransactionLogEntry(userId, basketId, itemId, action, source));
        context.SaveChanges();
        return Task.CompletedTask;
    }
}
