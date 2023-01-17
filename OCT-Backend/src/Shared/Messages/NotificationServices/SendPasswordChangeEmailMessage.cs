namespace Messages.NotificationServices;

public class SendPasswordChangeEmailMessage
{
    public string To { get; set; }

    public string ChangePasswordLink { get; set; }
}
