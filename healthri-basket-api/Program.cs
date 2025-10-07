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

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<ITransactionLogger, TransactionLoggerRepository>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IBasketService, BasketService>();

// PostgreSQL database setup (https://www.npgsql.org/efcore/index.html?tabs=onconfiguring)
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
builder.Services.AddDbContextPool<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata = false;
    // Optional if the MetadataAddress is specified
    jwtOptions.Authority = "http://localhost:8081/realms/healthri-basket-api";
    jwtOptions.Audience = "account";
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidAudiences = ["account"],
        ValidIssuers = ["http://localhost:8081/realms/healthri-basket-api"]
    }; 
    jwtOptions.MapInboundClaims = false;
});





builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



