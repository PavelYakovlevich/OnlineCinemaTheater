using Authentication.Core.Generators;
using FluentAssertions;

namespace AuthenticationService.Tests.Generators;

[TestFixture]
internal class EmailConfirmationKeyGeneratorTests
{
    [Test]
    public void GenerateToken_ReturnValidConfirmationKey()
    {
        // Arrange
        var appConfiguration = Fakers.ApplicationConfigurationFaker.Generate();
        var generator = new BCryptTokenGenerator(appConfiguration);
        var id = Guid.NewGuid();
        var expectedResult = BCrypt.Net.BCrypt.HashPassword(id.ToString(), appConfiguration.Salt);

        // Act
        var generatedToken = generator.GenerateToken(id);

        // Assert
        generatedToken.Should().Be(expectedResult);
    }
}
