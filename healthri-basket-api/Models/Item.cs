using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Models;

public class Item
{
    [Key]
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<BasketItem> Baskets { get; set; }

    public Item(Guid Id, string title, string description)
    {
        this.Id = Id;
        this.Title = title;
        this.Description = description;
    }

    public Item(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        Baskets = [];
    }
}

