using System.Security.Cryptography;

namespace Authentication.Core.Generators;

public static class RefreshTokenGenerator
{
    public static string GenerateToken(int tokenLength)
    {
        if (tokenLength < 1 || tokenLength > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(tokenLength), $"${nameof(tokenLength)} is less than 1 or greater than 1000.");
        }

        var randomNumber = new byte[tokenLength];
        using var random = RandomNumberGenerator.Create();
        random.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber)[..tokenLength];
    }
}
