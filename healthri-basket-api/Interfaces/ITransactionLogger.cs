using healthri_basket_api.Models.Enums;

namespace healthri_basket_api.Interfaces;

public interface ITransactionLogger
{
    Task LogAsync(Guid userId, Guid basketId, Guid itemId, BasketAction action, BasketItemSource source);
}