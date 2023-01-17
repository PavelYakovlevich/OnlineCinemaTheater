using Bogus;
using Exceptions.NotificationService;
using Notification.Email.SendGrid;

namespace NotificationService.Tests.Services.EmailSenders;

[TestFixture]
internal class SendGridEmailSenderTests
{
    private static readonly Faker Faker = new ();
    private readonly SendGridEmailSender _emailSender;

    private static IEnumerable<TestCaseData> InvalidStrings
    {
        get
        {
            yield return new TestCaseData(null);
            yield return new TestCaseData(string.Empty);
            yield return new TestCaseData(" ");
        }
    }

    public SendGridEmailSenderTests()
    {
        _emailSender = new SendGridEmailSender(Faker.Random.Hash());
    }

    [TestCaseSource(nameof(InvalidStrings))]
    public static void Ctor_InvalidKey_ThrowsArgumentException(string key) =>
        Assert.Throws<ArgumentException>(() => new SendGridEmailSender(key));

    [TestCaseSource(nameof(InvalidStrings))]
    public void SendEmailAsync_InvalidFromEmail_ThrowsArgumentException(string fromEmail)
    {
        // Arrange
        var toEmail = Faker.Internet.Email();
        var subject = Faker.Random.Words();
        var body = Faker.Random.Words();

        // Act
        // Assert
        Assert.ThrowsAsync<ArgumentException>(() => _emailSender.SendEmailAsync(fromEmail, subject, body, toEmail));
    }

    [TestCaseSource(nameof(InvalidStrings))]
    public void SendEmailAsync_InvalidToEmail_ThrowsArgumentException(string toEmail)
    {
        // Arrange
        var fromEmail = Faker.Internet.Email();
        var subject = Faker.Random.Words();
        var body = Faker.Random.Words();

        // Act
        // Assert
        Assert.ThrowsAsync<ArgumentException>(() => _emailSender.SendEmailAsync(fromEmail, subject, body, toEmail));
    }

    [TestCaseSource(nameof(InvalidStrings))]
    public void SendEmailAsync_InvalidSubject_ThrowsArgumentException(string subject)
    {
        // Arrange
        var fromEmail = Faker.Internet.Email();
        var toEmail = Faker.Internet.Email();
        var body = Faker.Random.Words();

        // Act
        // Assert
        Assert.ThrowsAsync<ArgumentException>(() => _emailSender.SendEmailAsync(fromEmail, subject, body, toEmail));
    }

    [TestCaseSource(nameof(InvalidStrings))]
    public void SendEmailAsync_InvalidBody_ThrowsArgumentException(string body)
    {
        // Arrange
        var fromEmail = Faker.Internet.Email();
        var toEmail = Faker.Internet.Email();
        var subject = Faker.Random.Words();

        // Act
        // Assert
        Assert.ThrowsAsync<ArgumentException>(() => _emailSender.SendEmailAsync(fromEmail, subject, body, toEmail));
    }

    [Test]
    public static void SendEmailAsync_InvalidAPIKey_ThrowsEmailSendUserException()
    {
        var sender = new SendGridEmailSender(Faker.Random.Hash());

        // Arrange
        var fromEmail = Faker.Internet.Email();
        var toEmail = Faker.Internet.Email();
        var subject = Faker.Random.Words();
        var body = Faker.Random.Words();

        // Act
        // Assert
        Assert.ThrowsAsync<EmailSendInternalException>(() => sender.SendEmailAsync(fromEmail, subject, body, toEmail));
    }
}