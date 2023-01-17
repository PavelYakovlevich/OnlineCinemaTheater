using Exceptions.NotificationService;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Notification.Contract;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;

namespace Notification.Email.SendGrid;

public class SendGridEmailSender : IEmailSender
{
    private readonly string _apiKey;

    public SendGridEmailSender(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException($"{nameof(apiKey)} is null empty or whitespace.", nameof(apiKey));
        }

        _apiKey = apiKey;
    }

    public async Task SendEmailAsync(string from, string subject, string body, string to)
    {
        if (string.IsNullOrEmpty(from) || string.IsNullOrWhiteSpace(from))
        {
            throw new ArgumentException($"{nameof(from)} is null empty or whitespace.", nameof(from));
        }

        if (string.IsNullOrEmpty(subject) || string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException($"{nameof(subject)} is null empty or whitespace.", nameof(subject));
        }

        if (string.IsNullOrEmpty(body) || string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException($"{nameof(body)} is null empty or whitespace.", nameof(body));
        }

        if (string.IsNullOrEmpty(to) || string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentException($"{nameof(to)} is null empty or whitespace.", nameof(to));
        }

        var client = new SendGridClient(new SendGridClientOptions
        {
            ApiKey = _apiKey,
            HttpErrorAsException = true,
        });

        var message = new SendGridMessage
        {
            From = new EmailAddress(from),
            Subject = subject,
            HtmlContent = body,
        };

        message.AddTo(new EmailAddress(to));

        try
        {
            await client.SendEmailAsync(message);
        }
        catch (Exception exception)
        {
            var errorResponse = JsonConvert.DeserializeObject<SendGridErrorResponse>(exception.Message)!;

            if (errorResponse.ErrorHttpStatusCode == StatusCodes.Status401Unauthorized ||
                errorResponse.ErrorHttpStatusCode >= StatusCodes.Status500InternalServerError)
            {
                throw new EmailSendInternalException(errorResponse.SendGridErrorMessage, exception);
            }

            throw new EmailSendUserException(errorResponse.SendGridErrorMessage, exception);
        }
    }
}