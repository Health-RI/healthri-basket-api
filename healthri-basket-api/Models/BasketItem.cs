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
    [Key]
    public Guid ItemId { get; set; } = new Guid();
    public string Source { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}

