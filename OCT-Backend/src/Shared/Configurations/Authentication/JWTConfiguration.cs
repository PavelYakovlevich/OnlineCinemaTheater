namespace Configurations.Authentication;

public class JWTConfiguration
{
    public string Key { get; set; }

    public int ExpiresInMinutes { get; set; }

    public string Issuer { get; set; }
}