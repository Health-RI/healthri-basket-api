using healthri_basket_api.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace healthri_basket_api.Models;

public class BasketItem
{
    [Key]
    public Guid Id { get; init; }
    public Guid BasketId { get; set; }
    public Basket Basket { get; set; } = null!;
    public Guid ItemId { get; set; }
    public Item Item { get; set; } = null!;
    public BasketItemStatus Status { get; set; }

    // Parameterless constructor for EF Core
    public BasketItem() { }

    public BasketItem(Basket basket, Item item)
    {
        Id = Guid.NewGuid();
        BasketId = basket.Id;
        Basket = basket;
        ItemId = item.Id;
        Item = item;
        Status = BasketItemStatus.InBasket;
    }
}
