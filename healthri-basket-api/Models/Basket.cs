using healthri_basket_api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Models;

public class Basket
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Slug { get; set; }
    public string Name { get; set; }
    public bool IsDefault { get; set; }
    public BasketStatus Status { get; set; }
    public List<BasketItem> Items { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public Basket(Guid userId, string slug, string name, bool isDefault) { 
        Id = Guid.NewGuid();
        UserId = userId;
        Slug = slug;
        Name = name;
        IsDefault = isDefault;
        Status = BasketStatus.Active;
        Items = [];
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

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

    public void Rename(string newName, string newSlug)
    {
        Name = newName;
        Slug = newSlug;
        UpdatedAt = DateTime.UtcNow;
    }
}