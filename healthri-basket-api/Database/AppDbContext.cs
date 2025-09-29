using healthri_basket_api.Models;
using Microsoft.EntityFrameworkCore;

namespace healthri_basket_api.Database;

public class AppDbContext : DbContext
{
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<TransactionLogEntry> TransactionLogs { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Basket -> BasketItems (Delete BasketItem on Basket deletion)
        modelBuilder.Entity<Basket>()
            .HasMany(b => b.Items)
            .WithOne(i => i.Basket)
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

        // Item -> BasketItems (Cascade delete)
        modelBuilder.Entity<Item>()
            .HasMany(i => i.Baskets)
            .WithOne(b => b.Item)
            .HasForeignKey(b => b.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // BasketItem -> Basket (Restrict BasketItem for Basket deletion)
        modelBuilder.Entity<BasketItem>()
            .HasOne(bi => bi.Basket)
            .WithMany(b => b.Items)
            .HasForeignKey(bi => bi.BasketId)
            .OnDelete(DeleteBehavior.Restrict);

        // BasketItem -> Item (Restrict BasketItem for item delete)
        modelBuilder.Entity<BasketItem>()
            .HasOne(bi => bi.Item)
            .WithMany(i => i.Baskets)
            .HasForeignKey(bi => bi.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed Items
        modelBuilder.Entity<Item>().HasData(
            new Item(Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            title: "NEOLOS - physiological measurements of preterm infants with and without late onset sepsis", 
            description: "Collection of patient monitoring data of premature infants, ECG, CI and parameters as SpO2 and Temp. Half of the infants experienced a period of late onset sepsis during their hospital stay, the other half does not."),

            new Item(Id: Guid.Parse("11111111-1111-1111-1111-222222222222"), 
            title: "COntrol of COVID-19 iN Hospitals - environmental study", 
            description: "Infectiepreventie van COVID-19 in ziekenhuizen - omgevingsstudie; COntrol of COVID-19 iN Hospitals - environmental study"),

            new Item(Id: Guid.Parse("11111111-1111-1111-1111-333333333333"), 
            title: "ctDNA on the way to implementation in the Netherlands (COIN)", 
            description:"ctDNA on the way to implementation in the Netherlands (COIN)")
        );
    }
}