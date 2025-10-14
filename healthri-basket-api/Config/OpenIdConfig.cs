namespace healthri_basket_api.Config;

public class OpenIdConfiguration
{
    public string Authority { get; set; } = default!;
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;

    public static OpenIdConfiguration LoadFromEnv()
    {

        var authority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY");
        var issuer = Environment.GetEnvironmentVariable("OPENID_ISSUER");
        var audience = Environment.GetEnvironmentVariable("OPENID_AUDIENCE");
        var clientId = Environment.GetEnvironmentVariable("OPENID_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("OPENID_CLIENT_SECRET");

        if (string.IsNullOrEmpty(authority) ||
            string.IsNullOrEmpty(issuer) ||
            string.IsNullOrEmpty(audience) ||
            string.IsNullOrEmpty(clientId) ||
            string.IsNullOrEmpty(clientSecret))
        {
            throw new Exception("One or more required OpenID environment variables are missing.");
        }

        return new OpenIdConfiguration
        {
            Authority = authority,
            Issuer = issuer,
            Audience = audience,
            ClientId = clientId,
            ClientSecret = clientSecret
        };
    }
}
