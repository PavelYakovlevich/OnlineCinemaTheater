using Authentication.API.Validators;
using Bogus;
using FluentValidation.TestHelper;
using Models.AuthenticationService.Request;

namespace AuthenticationService.Tests.Validators;

[TestFixture]
internal class ChangeAccountPasswordRequestValidatorTests
{
    private readonly ChangeAccountPasswordRequestValidator _validator = new ();
    private readonly Faker _faker = new ();

    [TestCaseSource(typeof(ValidatorsTestCases), nameof(ValidatorsTestCases.InvalidPasswords))]
    public async Task Validate_InvalidPassword_ValidationWasNotPassed(string password)
    {
        // Arrange
        var request = new ChangeAccountRequest { Password = password };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(_ => _.Password);
    }

    [Test]
    public async Task Validate_ValidPassword_ValidationWasPassed()
    {
        // Arrange
        var request = new ChangeAccountRequest { Password = _faker.Internet.Password() };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
