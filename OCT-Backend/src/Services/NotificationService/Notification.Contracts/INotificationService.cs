namespace Notification.Contract;

public interface INotificationService<in T>
{
    Task SendEmailConfirmationMailAsync(T notifyDetails);

    Task SendPasswordWasForgottenMailAsync(T details);
}