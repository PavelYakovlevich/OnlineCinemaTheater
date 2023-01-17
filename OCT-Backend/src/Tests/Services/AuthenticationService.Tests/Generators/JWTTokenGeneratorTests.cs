using Authentication.Core.Generators;
using Configurations.Authentication;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationService.Tests.Generators;

[TestFixtureSource(nameof(JwtConfigurations))]
internal class JWTTokenGeneratorTests
{
	private static IEnumerable<JWTConfiguration> JwtConfigurations
	{
		get
		{
			yield return Fakers.JWTConfigurationFaker.Generate();
		}
	}

	private static readonly string[] ExpectedClaims = new[]
	{
        ClaimTypes.NameIdentifier,
		ClaimTypes.Role,
		ClaimTypes.Email,
	};

	private readonly JWTTokenGenerator _generator;
	private readonly JWTConfiguration _configuration;
	private readonly JwtSecurityTokenHandler _tokenHandler;
	private readonly TokenValidationParameters _validationParameters;

	public JWTTokenGeneratorTests(JWTConfiguration configuration)
	{
        _generator = new JWTTokenGenerator(configuration);
		_configuration = configuration;

        _tokenHandler = new JwtSecurityTokenHandler();
        _validationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = _configuration.Issuer,

			ValidateLifetime = true,

			ValidateAudience = false,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(_configuration.Key))
		};
    }

	[Test]
    public void GenerateToken_ValidModel_TokenContainsAllClaims()
    {
		// Arrange
		var accountModel = Fakers.AccountFaker.Generate();
        var token = _generator.GenerateToken(accountModel);
		
		// Act
        var principal = _tokenHandler.ValidateToken(token, _validationParameters, out var _);
		var tokenClaims = principal.Identities.First().Claims.Select(c => c.Type);

		// Assert
		ExpectedClaims.Should().BeSubsetOf(tokenClaims);
    }

	[Test]
	public void GenerateToken_ValidModel_ReturnsValidToken()
	{
        // Arrange
        var accountModel = Fakers.AccountFaker.Generate();
		var token = _generator.GenerateToken(accountModel);

        // Act
        // Assert
        Assert.DoesNotThrow(() => 
			_tokenHandler.ValidateToken(token, _validationParameters, out var _), "Validation of key failed");
    }

	[Test]
    public void GenerateToken_Null_ThrowsArgumentNullException() =>
		Assert.Throws<ArgumentNullException>(() => _generator.GenerateToken(null));
}
