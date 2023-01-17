using Bogus;
using Moq;
using Notification.Contract;
using Notification.Core;
using Notification.Core.Services;

namespace NotificationService.Tests.Services;

[TestFixture]
internal class EmailNotificationServiceTests
{
    public static Faker<EmailDetails> EmailDetailsFaker = new Faker<EmailDetails>()
        .RuleFor(p => p.From, f => f.Internet.Email())
        .RuleFor(p => p.To, f => f.Internet.Email())
        .RuleFor(p => p.Subject, f => f.Random.Words())
        .RuleFor(p => p.Body, f => f.Random.Word());

    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly EmailNotificationService _service;

    public EmailNotificationServiceTests()
    {
        _emailSenderMock = new Mock<IEmailSender>();
        _service = new EmailNotificationService(_emailSenderMock.Object);
    }

    [Test]
    public void SendEmailConfirmationMailAsync_Null_ThrowsArgumentNullException() =>
        Assert.ThrowsAsync<ArgumentNullException>(() => _service.SendEmailConfirmationMailAsync(null));

    [Test]
    public async Task SendEmailConfirmationMailAsync_Calls_EmailSender_SendEmailAsync_Once()
    {
        // Arrange
        var emailDetails = EmailDetailsFaker.Generate();

        // Act
        await _service.SendEmailConfirmationMailAsync(emailDetails);

        // Assert
        _emailSenderMock.Verify(
            _ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Test]
    public void SendPasswordWasForgottenMailAsync_Null_ThrowsArgumentNullException() =>
        Assert.ThrowsAsync<ArgumentNullException>(() => _service.SendPasswordWasForgottenMailAsync(null));

    [Test]
    public async Task SendPasswordWasForgottenMailAsync_Calls_EmailSender_SendEmailAsync_Once()
    {
        // Arrange
        var emailDetails = EmailDetailsFaker.Generate();

        // Act
        await _service.SendPasswordWasForgottenMailAsync(emailDetails);

        // Assert
        _emailSenderMock.Verify(
            _ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [TearDown]
    public void Teardown()
    {
        _emailSenderMock.Invocations.Clear();
    }
}
