using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<TransactionLogEntry> TransactionLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique constraint on UserId + Slug
        modelBuilder.Entity<Basket>()
            .HasIndex(b => new { b.UserId, b.Slug })
            .IsUnique();

        // Basket -> BasketItems (Delete BasketItem on Basket deletion)
        modelBuilder.Entity<Basket>()
            .HasMany(b => b.Items)
            .WithOne(i => i.Basket)
            .HasForeignKey(i => i.BasketId);

        // BasketItem -> Basket (Restrict BasketItem for Basket deletion)
        modelBuilder.Entity<BasketItem>()
            .HasOne(bi => bi.Basket)
            .WithMany(b => b.Items)
            .HasForeignKey(bi => bi.BasketId);

        modelBuilder.Entity<Item>()
            .Property(i => i.Id)
            .HasMaxLength(512);

        modelBuilder.Entity<BasketItem>()
            .Property(bi => bi.ItemId)
            .HasMaxLength(512);

        modelBuilder.Entity<TransactionLogEntry>()
            .Property(tl => tl.ItemId)
            .HasMaxLength(512);
    }
}
