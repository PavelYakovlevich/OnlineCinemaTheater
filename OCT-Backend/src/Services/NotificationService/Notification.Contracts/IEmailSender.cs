namespace Notification.Contract;

public interface IEmailSender
{
    Task SendEmailAsync(string from, string subject, string body, string to);
}
