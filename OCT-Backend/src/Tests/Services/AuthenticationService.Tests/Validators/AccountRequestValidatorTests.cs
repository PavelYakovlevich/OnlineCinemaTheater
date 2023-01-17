using Authentication.API.Validators;
using FluentValidation.TestHelper;

namespace AuthenticationService.Tests.Validators;

[TestFixture]
internal class AccountRequestValidatorTests
{
    private readonly AccountRequestValidator _validator = new ();

    [TestCaseSource(typeof(ValidatorsTestCases), nameof(ValidatorsTestCases.InvalidEmails))]
    public async Task Validate_InvalidEmail_ValidationWasNotPassed(string email)
	{
		// Arrange
		var request = Fakers.AccountRequestFaker.Generate();
		request.Email = email;

		// Act
		var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(_ => _.Email);

    }

    [TestCaseSource(typeof(ValidatorsTestCases), nameof(ValidatorsTestCases.InvalidPasswords))]
    public async Task Validate_InvalidPassword_ValidationWasNotPassed(string password)
    {
        // Arrange
        var request = Fakers.AccountRequestFaker.Generate();
        request.Password = password;

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(_ => _.Password);
    }

    [Test]
    public async Task Validate_ValidEmailAndPassword_ValidationWasPassed()
    {
        // Arrange
        var request = Fakers.AccountRequestFaker.Generate();

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
