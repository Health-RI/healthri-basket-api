namespace healthri_basket_api.Interfaces;

public interface ITransactionLogger
{
    Task LogAsync(Guid userUuid, Guid basketId, string itemId, string action, string source);
}