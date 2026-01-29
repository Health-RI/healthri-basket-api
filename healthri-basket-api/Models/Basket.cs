using healthri_basket_api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Models;

public class Basket(Guid userId, string name, bool isDefault)
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; } = userId;
    public string Name { get; set; } = name;
    public bool IsDefault { get; set; } = isDefault;
    public BasketStatus Status { get; set; } = BasketStatus.Active;
    public List<BasketItem> Items { get; set; } = [];
    public DateTime? DeletedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public void AddItem(Item item)
    {
        var basketItem = new BasketItem(this, item);
        Items.Add(basketItem);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid itemId)
    {
        BasketItem? basketItem = this.Items.FirstOrDefault(i => i.ItemId.Equals(itemId));
        if (basketItem == null){
            return;
        }
                    
        Items.Remove(basketItem);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasItem(Guid itemId)
    {
        return this.Items.Any(bi => bi.ItemId.Equals(itemId));
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