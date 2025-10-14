using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Models;

public class TransactionLogEntry
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid BasketId { get; set; }
    public Guid? ItemId { get; set; }
    public BasketAction Action { get; set; }
    public BasketItemSource Source { get; set; }
    public DateTime Timestamp { get; set; }

    public TransactionLogEntry(Guid userId, Guid basketId, Guid? itemId, BasketAction action, BasketItemSource source)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        BasketId = basketId;
        ItemId = itemId;
        Action = action;
        Source = source;
        Timestamp = DateTime.UtcNow;
    }
}