using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Item?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        }

    }
}
