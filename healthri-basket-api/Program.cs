using dotenv.net;
using healthri_basket_api.Config;
using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Repositories;
using healthri_basket_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

// Initialize config from environment variables
DotEnv.Load();
var dbConfig = DatabaseConfig.LoadFromEnv();
var openIdConfig = OpenIdConfiguration.LoadFromEnv();

// PostgreSQL setup
builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql(dbConfig.ConnectionString));

// JWT Authentication setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata = false;
    jwtOptions.Authority = openIdConfig.Authority;
    jwtOptions.Audience = openIdConfig.Audience;
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudiences = [openIdConfig.Audience],
        ValidIssuers = [openIdConfig.Issuer]
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