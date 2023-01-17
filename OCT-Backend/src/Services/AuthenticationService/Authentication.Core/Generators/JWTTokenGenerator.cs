using Authentication.Domain.Models;
using Configurations.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Core.Generators;

public class JWTTokenGenerator
{
    private readonly JWTConfiguration _configuration;

    public JWTTokenGenerator(JWTConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(AccountModel accountModel)
    {
        accountModel = accountModel ?? throw new ArgumentNullException(nameof(accountModel));

        var tokenHandler = new JwtSecurityTokenHandler();

        var keyBytes = Encoding.UTF8.GetBytes(_configuration.Key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.NameIdentifier, accountModel.Id.ToString()),
                new (ClaimTypes.Email, accountModel.Email),
                new (ClaimTypes.Role, accountModel.Role.ToString()),
            }),
            Issuer = _configuration.Issuer,
            Expires = DateTime.UtcNow.AddMinutes(_configuration.ExpiresInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
        };

        return tokenHandler.CreateEncodedJwt(tokenDescriptor);
    }
}
