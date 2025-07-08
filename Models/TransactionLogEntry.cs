namespace healthri_basket_api.Models;

public class TransactionLogEntry
{
    public Guid UserUuid { get; set; }
    public Guid BasketId { get; set; }
    public string ItemId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}