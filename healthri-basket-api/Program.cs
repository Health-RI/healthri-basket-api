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
    options.LoginPath = "/Account/Login";
})

.AddOpenIdConnect(options =>
{
    options.Authority = builder.Configuration.GetSection("OpenID")["Authority"];
    options.ClientId = builder.Configuration.GetSection("OpenID")["ClientId"]; ;
    options.ClientSecret = builder.Configuration.GetSection("OpenID")["ClientSecret"]; ; // client authentication must be enabled
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.CallbackPath = "/swagger/index.html";
    options.SignedOutCallbackPath = "/swagger/index.html";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = "roles"
    };

    // For development only - disable HTTPS metadata validation
    // In production, use explicit Authority configuration instead
    if (builder.Environment.IsDevelopment())
    {
        options.RequireHttpsMetadata = false;
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



