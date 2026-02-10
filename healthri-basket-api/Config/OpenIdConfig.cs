namespace healthri_basket_api.Config;

public class OpenIdConfiguration
{
    public required string Authority { get; init; } 
    public required string Issuer { get; init; } 
    public required string Audience { get; init; }

    public static OpenIdConfiguration LoadFromEnv()
    {

        var authority = Environment.GetEnvironmentVariable("OPENID_AUTHORITY");
        var issuer = Environment.GetEnvironmentVariable("OPENID_ISSUER");
        var audience = Environment.GetEnvironmentVariable("OPENID_AUDIENCE");

        if (string.IsNullOrEmpty(authority) ||
            string.IsNullOrEmpty(issuer) ||
            string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("One or more required OpenID environment variables are missing.");
        }

        return new OpenIdConfiguration
        {
            Authority = authority,
            Issuer = issuer,
            Audience = audience,
        };
    }
}
