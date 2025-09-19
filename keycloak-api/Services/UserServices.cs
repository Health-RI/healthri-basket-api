using keycloak_api.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace keycloak_api.Services;

public class UserService : IUserService
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public UserService(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    public Task<string> RegisterAsync(RegisterRequestDTO request)
    {
        var realm = "healthri-basket-api";
        var keycloakUrl = "http://localhost:8080";
    }

    public Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var realm = "healthri-basket-api";
        var keycloakUrl = "http://localhost:8080";
    }
}
