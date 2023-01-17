namespace Messages.NotificationServices;

public class SendConfirmationEmailMessage
{
    public string To { get; set; }

    public string ConfirmationLink { get; set; }
}
