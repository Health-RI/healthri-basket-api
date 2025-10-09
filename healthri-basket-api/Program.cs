using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Repositories;
using healthri_basket_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using dotenv.net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency injection
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ITransactionLogger, TransactionLoggerRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IBasketService, BasketService>();

// Get environment variables
DotEnv.Load();
string? dbConnectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_URL");
string? openIdAuthority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY");
string? openIdIssuer = Environment.GetEnvironmentVariable("OPENID_ISSUER");
string? openIdAudience = Environment.GetEnvironmentVariable("OPENID_AUDIENCE");
string? openIdClientId = Environment.GetEnvironmentVariable("OPENID_CLIENT_ID");
string? openIdClientSecret = Environment.GetEnvironmentVariable("OPENID_CLIENT_SECRET");

if (string.IsNullOrEmpty(dbConnectionString))
    throw new Exception("POSTGRESQL_CONNECTION_URL environment variable is not set.");
if (string.IsNullOrEmpty(openIdAuthority))
    throw new Exception("OPENID_AUTHORITY environment variable is not set.");
if (string.IsNullOrEmpty(openIdIssuer))
    throw new Exception("OPENID_ISSUER environment variable is not set.");
if (string.IsNullOrEmpty(openIdAudience))
    throw new Exception("OPENID_AUDIENCE environment variable is not set.");
if (string.IsNullOrEmpty(openIdClientId))
    throw new Exception("OPENID_CLIENT_ID environment variable is not set.");
if (string.IsNullOrEmpty(openIdClientSecret))
    throw new Exception("OPENID_CLIENT_SECRET environment variable is not set.");

// PostgreSQL setup
builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql(dbConnectionString));

// JWT Authentication setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata = false;
    jwtOptions.Authority = openIdAuthority;
    jwtOptions.Audience = openIdAudience;
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudiences = [openIdAudience],
        ValidIssuers = [openIdIssuer]
    }; 
    jwtOptions.MapInboundClaims = false;
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();