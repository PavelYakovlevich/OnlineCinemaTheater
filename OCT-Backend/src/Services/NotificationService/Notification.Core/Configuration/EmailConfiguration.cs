namespace Notification.Core.Configuration
{
    public class EmailConfiguration
    {
        public string SenderSource { get; set; }

        public string SenderEmail { get; set; }

        public string EmailConfirmationSubject { get; set; }

        public string ChangePasswordSubject { get; set; }

        public MailKitConfiguration MailKit { get; set; }

        public SendGridConfiguration SendGrid { get; set; }
    }
}
