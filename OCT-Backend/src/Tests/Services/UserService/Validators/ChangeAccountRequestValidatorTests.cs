using FluentValidation.TestHelper;
using User.API.Validators;

namespace UserService.Tests.Validators;

[TestFixture]
internal class ChangeAccountRequestValidatorTests
{
    private static IEnumerable<TestCaseData> InvalidBirthdays
    {
        get
        {
            yield return new TestCaseData(DateTime.UtcNow.AddMinutes(1));
            yield return new TestCaseData(DateTime.UtcNow.AddDays(1));
            yield return new TestCaseData(DateTime.UtcNow.AddMonths(1));
            yield return new TestCaseData(DateTime.UtcNow.AddYears(1));
        }
    }

    private static IEnumerable<TestCaseData> InvalidNames
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData(string.Empty);
            yield return new TestCaseData(" ");
            yield return new TestCaseData("Name1");
            yield return new TestCaseData("Name@");
            yield return new TestCaseData("too_long_name_rewfdfsafsafsdafsdfsfgjasgfhjasgfqwee");
        }
    }

    private static IEnumerable<TestCaseData> InvalidSurnames
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData(string.Empty);
            yield return new TestCaseData(" ");
            yield return new TestCaseData("Surname1");
            yield return new TestCaseData("Surname@");
            yield return new TestCaseData("too_long_surname_rewfdfsafsafsdafsfgjasgfhjasgfqwee");
        }
    }

    private static IEnumerable<TestCaseData> InvalidMiddlenames
    {
        get
        {
            yield return new TestCaseData(string.Empty);
            yield return new TestCaseData(" ");
            yield return new TestCaseData("Middlename1");
            yield return new TestCaseData("Middlename@");
            yield return new TestCaseData("too_long_middle_name_refdfsafsdafsfgjasgfhjasgfqwee");
        }
    }

    private readonly ChangeUserRequestValidator _validator = new ();

    [TestCaseSource(nameof(InvalidNames))]
    public async Task Validate_InvalidName_ValidationWasNotPassed(string name)
    {
        // Arrange
        var request = Fakers.ChangeRequestFaker.Generate();
        request.Name = name;

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(_ => _.Name);
    }

    [TestCaseSource(nameof(InvalidSurnames))]
    public async Task Validate_InvalidSurname_ValidationWasNotPassed(string surname)
    {
        // Arrange
        var request = Fakers.ChangeRequestFaker.Generate();
        request.Surname = surname;

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(_ => _.Surname);
    }

    [TestCaseSource(nameof(InvalidMiddlenames))]
    public async Task Validate_InvalidMiddlename_ValidationWasNotPassed(string middlename)
    {
        // Arrange
        var request = Fakers.ChangeRequestFaker.Generate();
        request.MiddleName = middlename;

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(_ => _.MiddleName);
    }

    [TestCaseSource(nameof(InvalidBirthdays))]
    public async Task Validate_InvalidBirthday_ValidationWasNotPassed(DateTime birthday)
    {
        // Arrange
        var request = Fakers.ChangeRequestFaker.Generate();
        request.Birthday = birthday;

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldHaveValidationErrorFor(_ => _.Birthday);
    }

    [Test]
    public async Task Validate_ValidChangeRequestObject_ValidationPassed()
    {
        // Arrange
        var request = Fakers.ChangeRequestFaker.Generate();

        // Act
        var validationResult = await _validator.TestValidateAsync(request);

        // Assert
        validationResult.ShouldNotHaveAnyValidationErrors();
    }
}
