using Comment.API.Validators;
using FluentValidation.TestHelper;

namespace CommentService.Tests.Validators;

[TestFixture]
internal class UpdateCommentRequestValidatorTests
{
    private readonly UpdateCommentRequestValidator _validator = new();


    [Test]
    public async Task Validate_InvalidText_ValidationWasNotPassed()
    {
        // Arrange
        var request = Fakers.UpdateCommentRequestFaker.Generate();
        request.Text = string.Empty;

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(_ => _.Text);
    }

    [Test]
    public async Task Validate_ValidRequestData_ValidationWasPassed()
    {
        // Arrange
        var request = Fakers.UpdateCommentRequestFaker.Generate();

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
