using healthri_basket_api.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace healthri_basket_api;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        // Use connection string matching docker-compose.yaml for migrations
        optionsBuilder.UseNpgsql("Host=localhost;Database=healthri_basket_api;Username=test;Password=test");

        return new AppDbContext(optionsBuilder.Options);
    }
}
