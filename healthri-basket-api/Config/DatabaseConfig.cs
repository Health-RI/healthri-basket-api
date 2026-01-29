namespace healthri_basket_api.Config;

public class DatabaseConfig
{
    public string? ConnectionString { get; set; }

    public static DatabaseConfig LoadFromEnv()
    {
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new Exception("Database connection string environment variable is missing.");
        }
        return new DatabaseConfig
        {
            ConnectionString = connectionString
        };
    }
}
