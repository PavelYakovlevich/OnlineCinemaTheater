using Comment.API.Validators;
using FluentValidation.TestHelper;

namespace CommentService.Tests.Validators;

[TestFixture]
internal class GetCommentsFiltersValidatorTests
{
    private static IEnumerable<TestCaseData> InvalidOffsets
    {
        get
        {
            yield return new TestCaseData(int.MinValue);
            yield return new TestCaseData(-1);
        }
    }
    private static IEnumerable<TestCaseData> InvalidLimits
    {
        get
        {
            yield return new TestCaseData(int.MinValue);
            yield return new TestCaseData(-1);
            yield return new TestCaseData(0);
        }
    }

    private readonly GetCommentsFiltersValidator _validator = new();

    [TestCaseSource(nameof(InvalidOffsets))]
    public async Task Validate_InvalidOffset_ValidationWasNotPassed(int offset)
    {
        // Arrange
        var request = Fakers.GetCommentsFiltersFaker.Generate();
        request.Offset = offset;

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(_ => _.Offset);
    }

    [TestCaseSource(nameof(InvalidLimits))]
    public async Task Validate_InvalidLimit_ValidationWasNotPassed(int limit)
    {
        // Arrange
        var request = Fakers.GetCommentsFiltersFaker.Generate();
        request.Limit = limit;

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(_ => _.Limit);
    }

    [Test]
    public async Task Validate_ValidRequestData_ValidationWasPassed()
    {
        // Arrange
        var request = Fakers.GetCommentsFiltersFaker.Generate();

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
