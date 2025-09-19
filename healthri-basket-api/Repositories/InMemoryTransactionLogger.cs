using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;

namespace healthri_basket_api.Repositories;

public class InMemoryTransactionLogger : ITransactionLogger
{
    private readonly List<TransactionLogEntry> _entries = [];

    public Task LogAsync(Guid userUuid, Guid basketId, string itemId, string action, string source)
    {
        _entries.Add(new TransactionLogEntry
        {
            UserUuid = userUuid,
            BasketId = basketId,
            ItemId = itemId,
            Action = action,
            Source = source,
            Timestamp = DateTime.UtcNow
        });

        return Task.CompletedTask;
    }
}

