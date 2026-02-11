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
    public Guid BasketId { get; init; }
    public required Basket Basket{ get; init; }
    public Guid ItemId { get; init; }
    public required Item Item{ get; init; }
    public BasketItemStatus Status { get; init; }
}
