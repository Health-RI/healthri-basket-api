using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options){}
    }
}
