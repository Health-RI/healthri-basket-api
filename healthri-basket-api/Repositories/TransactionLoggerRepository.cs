using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Repositories;

public class TransactionLoggerRepository : ITransactionLogger
{
    private readonly AppDbContext _context;

    public TransactionLoggerRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task LogAsync(Guid userUuid, Guid basketId, Guid itemId, BasketAction action, BasketItemSource source)
    {
        _context.Add(new TransactionLogEntry(userUuid, basketId, itemId, action, source));
        _context.SaveChanges();
        return Task.CompletedTask;
    }
}

