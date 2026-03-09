using healthri_basket_api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace healthri_basket_api.Models;

public class BasketItem
{
    // Parameterless constructor for EF Core
    private BasketItem() { }
    
    [SetsRequiredMembers]
    public BasketItem(Basket basket, string itemId)
    {
        Id = Guid.NewGuid();
        BasketId = basket.Id;
        Basket = basket;
        ItemId = itemId;
        Status = BasketItemStatus.InBasket;
    }
    
    [Key]
    public Guid Id { get; init; }
    public Guid BasketId { get; init; }
    public required Basket Basket{ get; init; }
    public required string ItemId { get; init; }
    public BasketItemStatus Status { get; init; }
}
