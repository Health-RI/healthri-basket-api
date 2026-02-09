using dotenv.net;
using healthri_basket_api.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace healthri_basket_api.Database;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        DotEnv.Load();
        var dbConfig = DatabaseConfig.LoadFromEnv();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(dbConfig.ConnectionString);

        return new AppDbContext(optionsBuilder.Options);
    }
}
