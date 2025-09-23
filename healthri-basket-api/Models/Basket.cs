using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Models;

public class Basket
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserUuid { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public BasketStatus Status { get; set; } = BasketStatus.Active;
    public List<BasketItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public void AddItem(BasketItem item)
    {
        if (Items.All(i => i.ItemId != item.ItemId))
        {
            Items.Add(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null)
        {
            Items.Remove(item);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ClearItems()
    {
        Items.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Archive()
    {
        Status = BasketStatus.Archived;
        ArchivedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Delete()
    {
        Status = BasketStatus.Deleted;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        Status = BasketStatus.Active;
        ArchivedAt = null;
        DeletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Rename(string newName)
    {
        Name = newName;
        UpdatedAt = DateTime.UtcNow;
    }
}