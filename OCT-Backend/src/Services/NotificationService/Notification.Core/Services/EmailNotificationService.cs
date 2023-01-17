using Enums.NotificationService;
using Notification.Contract;
using Stubble.Core.Builders;
using System.Reflection;

namespace Notification.Core.Services;

public class EmailNotificationService : INotificationService<EmailDetails>
{
    private readonly IEmailSender _emailSender;

    public EmailNotificationService(IEmailSender emailSender)
    {
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
    }

    public async Task SendEmailConfirmationMailAsync(EmailDetails notifyDetails)
    {
        _ = notifyDetails ?? throw new ArgumentNullException(nameof(notifyDetails));

        var emailBody = await GetRenderedPageAsync(EmailConfirmationTemplate.EmailConfirmationMail, new { link = notifyDetails.Body });

        await _emailSender.SendEmailAsync(notifyDetails.From, notifyDetails.Subject, emailBody, notifyDetails.To);
    }

    public async Task SendPasswordWasForgottenMailAsync(EmailDetails details)
    {
        _ = details ?? throw new ArgumentNullException(nameof(details));

        var emailBody = await GetRenderedPageAsync(EmailConfirmationTemplate.ChangePasswordMail, new { link = details.Body });

        await _emailSender.SendEmailAsync(details.From, details.Subject, emailBody, details.To);
    }

    private static async Task<string> GetRenderedPageAsync(EmailConfirmationTemplate template, object parameters)
    {
        var renderer = new StubbleBuilder()
            .Build();

        var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        using var streamReader = new StreamReader(Path.Combine(currentDirectory, "Resources", $"{template.ToString()}.Mustache"));

        var templatePage = await streamReader.ReadToEndAsync();

        return await renderer.RenderAsync(templatePage, parameters);
    }
}
