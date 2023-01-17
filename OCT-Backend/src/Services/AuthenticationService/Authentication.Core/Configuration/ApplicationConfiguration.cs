using Configurations.Authentication;

namespace Authentication.Core.Configuration;

public class ApplicationConfiguration
{
    public string Salt { get; set; }

    public int TokenLength { get; set; }

    public int RefreshTokenExpirationTimeInMinutes { get; set; }

    public JWTConfiguration JwtConfiguration { get; set; }
}
