using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Repositories
{
    public class ItemRepository(AppDbContext context) : IItemRepository
    {
        public Task<Item?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return context.Items.FirstOrDefaultAsync(i => i.Id == id, cancellationToken: ct);
        }

    }
}
