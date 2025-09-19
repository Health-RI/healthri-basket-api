using System.Text.Json.Serialization;

namespace keycloak_api.DTOs;

public class TokenResponseDTO
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}