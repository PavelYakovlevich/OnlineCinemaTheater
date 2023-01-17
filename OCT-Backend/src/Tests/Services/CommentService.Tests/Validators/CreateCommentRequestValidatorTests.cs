using Comment.API.Validators;
using FluentValidation.TestHelper;

namespace CommentService.Tests.Validators;

[TestFixture]
internal class CreateCommentRequestValidatorTests
{
	private readonly CreateCommentRequestValidator _validator = new ();

	[Test]
	public async Task Validate_InvalidText_ValidationWasNotPassed()
	{
		// Arrange
		var request = Fakers.CreateCommentRequestFaker.Generate();
		request.Text = string.Empty;

		// Act
        var validationResult = await _validator.TestValidateAsync(request);

		// Assert
		validationResult.ShouldHaveValidationErrorFor(_ => _.Text);
	}

    [Test]
    public async Task Validate_ValidText_ValidationWasPassed()
    {
        // Arrange
        var request = Fakers.CreateCommentRequestFaker.Generate();

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
