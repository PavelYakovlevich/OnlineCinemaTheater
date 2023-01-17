using Configurations.MessageBroker;
using Infrastructure.MessageBroker.Abstractions;
using MassTransit;
using Messages.UserService;

namespace Infrastructure.MessageBroker.Implementations;

public class RabbitMqUserPublishService : IUserPublishService
{
    private readonly ISendEndpointProvider _provider;

    public RabbitMqUserPublishService(ISendEndpointProvider provider)
    {
        _provider = provider;
    }

    public async Task PublishUserCreatedMessage(Guid id)
    {
        var message = new UserCreatedMessage
        {
            Id = id,
        };

        await PublishMessageAsync(message, MessageBrokerQueues.UserCreatedQueue);
    }

    public async Task PublishUserUpdatedMessage(Guid id, string name, string surname)
    {
        var message = new UserUpdatedMessage
        {
            UserId = id,
            Name = name,
            Surname = surname
        };

        await PublishMessageAsync(message, MessageBrokerQueues.UserUpdatedQueue);
    }

    private async Task PublishMessageAsync<T>(T message, string queueName)
        where T : class
    {
        var sendEndPoint = await _provider.GetSendEndpoint(new Uri($"queue:{queueName}"));

        await sendEndPoint.Send(message);
    }
}
