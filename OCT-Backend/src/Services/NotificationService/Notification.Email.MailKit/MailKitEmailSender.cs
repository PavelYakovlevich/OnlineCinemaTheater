using Exceptions.NotificationService;
using MailKit.Net.Smtp;
using MimeKit;
using Notification.Contract;

namespace Notification.Email.MailKit;

public class MailKitEmailSender : IEmailSender
{
    private readonly string _login;
    private readonly string _password;
    private readonly string _senderName;

    public MailKitEmailSender(string login, string password, string senderName)
    {
        if (string.IsNullOrEmpty(senderName) || string.IsNullOrWhiteSpace(senderName))
        {
            throw new ArgumentException($"{nameof(senderName)} is null or empty or whitespace", nameof(senderName));
        }

        _login = login;
        _password = password;
        _senderName = senderName;
    }

    public string SenderName => _senderName;

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

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(SenderName, from));
        email.To.Add(new MailboxAddress(to.Split('@')[0], to));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = body
        };

        using var client = new SmtpClient();

        const string smtpUrl = "smtp.yandex.ru";
        const int smtpPort = 465;

        try
        {
            await client.ConnectAsync(smtpUrl, smtpPort, true);
            await client.AuthenticateAsync(_login, _password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
        catch (Exception exception)
        {
            throw new EmailSendInternalException(exception.Message, exception);
        }
    }
}