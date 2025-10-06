using healthri_basket_api.Database;
using healthri_basket_api.Interfaces;
using healthri_basket_api.Repositories;
using healthri_basket_api.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

// Authentication setup (https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/openid-connect?view=aspnetcore-7.0)
// https://medium.com/@ahmed.gaduo_93938/how-to-implement-keycloak-authentication-in-a-net-core-application-ce8603698f24
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/api/v1/baskets/auth";
})
.AddOpenIdConnect(options =>
{
    options.Authority = "http://host.docker.internal:8081/realms/healthri-basket-api";
    options.ClientId = "healthri-basket-api";
    options.ClientSecret = "OX8hfXF6iY3UE5lkbEw165mbDrSXDyOK";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.CallbackPath = "/api/v1/baskets/auth";
    options.SignedOutCallbackPath = "/api/v1/baskets/auth";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = "roles"
    };
    options.RequireHttpsMetadata = false; // only for development
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



