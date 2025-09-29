using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id, CancellationToken ct);   
}