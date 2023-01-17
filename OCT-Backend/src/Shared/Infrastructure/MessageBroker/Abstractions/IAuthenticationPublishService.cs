namespace Infrastructure.MessageBroker.Abstractions;

public interface IAuthenticationPublishService
{
    Task PublishSendConfirmationEmailMessage(string recepientEmail, string token);

    Task PublishAccountCreatedMessage(Guid accountId);

    Task PublishSendPasswordChangeEmailMessage(string recepientEmail, Guid id, string token);
}
