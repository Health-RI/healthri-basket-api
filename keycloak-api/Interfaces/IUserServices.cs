using keycloak_api.DTOs;

namespace keycloak_api.Services;

public interface IUserService
{
    Task<string> RegisterAsync(RegisterRequestDTO request);
    Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request);
}