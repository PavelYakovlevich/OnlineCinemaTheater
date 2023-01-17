using Authentication.API.Validators;
using Bogus;
using FluentValidation.TestHelper;
using Models.AuthenticationService.Request;

namespace AuthenticationService.Tests.Validators;

[TestFixture]
internal class SendChangePasswordEmailRequestValidatorTest
{
    private readonly SendChangePasswordEmailRequestValidator _validator = new ();
    private readonly Faker _faker = new ();

    [TestCaseSource(typeof(ValidatorsTestCases), nameof(ValidatorsTestCases.InvalidEmails))]
    public async Task Validate_InvalidEmail_ValidationWasNotPassed(string email)
    {
        // Arrange
        var request = new SendChangePasswordEmailRequest { Email = email };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(_ => _.Email);
    }

    [Test]
    public async Task Validate_ValidEmail_ValidationWasPassed()
    {
        // Arrange
        var request = new SendChangePasswordEmailRequest { Email = _faker.Internet.Email() };

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
