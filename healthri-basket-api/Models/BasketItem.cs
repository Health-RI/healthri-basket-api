using healthri_basket_api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace healthri_basket_api.Models;

public class BasketItem
{
    // Parameterless constructor for EF Core
    public BasketItem() { }
    
    [SetsRequiredMembers]
    public BasketItem(Basket basket, Item item)
    {
        Id = Guid.NewGuid();
        BasketId = basket.Id;
        Basket = basket;
        ItemId = item.Id;
        Item = item;
        Status = BasketItemStatus.InBasket;
    }
    
    [Key]
    public Guid Id { get; init; }
    public Guid BasketId { get; set; }
    public required Basket Basket{ get; set; }
    public Guid ItemId { get; set; }
    public required Item Item{ get; set; }
    public BasketItemStatus Status { get; set; }
}
