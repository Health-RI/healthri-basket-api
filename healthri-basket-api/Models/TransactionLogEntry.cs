using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Models;

public class TransactionLogEntry(Guid userId, Guid basketId, Guid? itemId, BasketAction action, BasketItemSource source)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = userId;
    public Guid BasketId { get; set; } = basketId;
    public Guid? ItemId { get; set; } = itemId;
    public BasketAction Action { get; set; } = action;
    public BasketItemSource Source { get; set; } = source;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}