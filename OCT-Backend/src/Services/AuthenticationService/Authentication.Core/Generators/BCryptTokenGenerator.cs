using Authentication.Contract.Generators;
using Authentication.Core.Configuration;

using Crypt = BCrypt.Net.BCrypt;

namespace Authentication.Core.Generators;

public class BCryptTokenGenerator : ITokenGenerator<Guid>
{
    private readonly string _salt;

    public BCryptTokenGenerator(ApplicationConfiguration applicationConfiguration)
    {
        _salt = applicationConfiguration.Salt;
    }

    public bool ValidateToken(Guid id, string token) => Crypt.Verify(id.ToString(), token);

    public string GenerateToken(Guid id) => Crypt.HashPassword(id.ToString(), _salt);
}
