using Configurations.FrontendRoutes;
using Configurations.MessageBroker;
using Infrastructure.MessageBroker.Abstractions;
using MassTransit;
using Messages.AuthenticationService;
using Messages.NotificationServices;

namespace Infrastructure.MessageBroker.Implementations;

public class RabbitMqAuthenticationPublishService : IAuthenticationPublishService
{
    private readonly ISendEndpointProvider _provider;

    public RabbitMqAuthenticationPublishService(ISendEndpointProvider provider)
    {
        _provider = provider;
    }

    public async Task PublishAccountCreatedMessage(Guid accountId)
    {
        var message = new CreateUserMessage { AccountId = accountId };

        await PublishMessageAsync(message, MessageBrokerQueues.AccountCreatedQueue);
    }

    public async Task PublishSendConfirmationEmailMessage(string recepientEmail, string token)
    {
        var message = new SendConfirmationEmailMessage
        {
            To = recepientEmail,
            ConfirmationLink = $"{FrontendRoutes.ConfirmEmail}?token={token}"
        };

        await PublishMessageAsync(message, MessageBrokerQueues.ConfirmEmailQueue);
    }

    public async Task PublishSendPasswordChangeEmailMessage(string recepientEmail, Guid id, string token)
    {
        var message = new SendPasswordChangeEmailMessage
        {
            To = recepientEmail,
            ChangePasswordLink = $"{FrontendRoutes.Host}/{id}/change-password?token={token}",
        };

        await PublishMessageAsync(message, MessageBrokerQueues.ChangePasswordEmailQueue);
    }

    private async Task PublishMessageAsync<T>(T message, string queueName)
        where T : class
    {
        var sendEndPoint = await _provider.GetSendEndpoint(new Uri($"queue:{queueName}"));

        await sendEndPoint.Send(message);
    }
}
