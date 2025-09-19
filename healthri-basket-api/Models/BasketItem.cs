using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Models;

public enum BasketStatus
{
    Active,
    Archived,
    Deleted
}

public class BasketItem
{
    public string ItemId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

