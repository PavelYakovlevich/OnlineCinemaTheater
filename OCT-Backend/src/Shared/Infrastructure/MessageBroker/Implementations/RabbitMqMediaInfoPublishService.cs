using Configurations.MessageBroker;
using Infrastructure.MessageBroker.Abstractions;
using MassTransit;
using Messages.MediaInfoService;

namespace Infrastructure.MessageBroker.Implementations;

public class RabbitMqMediaInfoPublishService : IMediaInfoPublishService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public RabbitMqMediaInfoPublishService(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task PublishMediaInfoCreatedMessage(Guid id, bool isTvSerias, bool isVisible)
    {
        var message = new MediaInfoCreatedMessage
        {
            Id = id,
            IsTvSerias = isTvSerias,
            IsVisible = isVisible
        };

        await PublishMessageAsync(message, MessageBrokerQueues.MediaInfoCreatedQueue);
    }

    public async Task PublishMediaInfoDeletedMessage(Guid id)
    {
        var message = new MediaInfoDeletedMessage
        {
            Id = id,
        };

        await PublishMessageAsync(message, MessageBrokerQueues.MediaInfoDeletedQueue);
    }

    public async Task PublishMediaInfoUpdatedMessage(Guid id, bool isTvSerias, bool isVisible)
    {
        var message = new MediaInfoUpdatedMessage
        {
            Id = id,
            IsTvSerias = isTvSerias,
            IsVisible = isVisible
        };

        await PublishMessageAsync(message, MessageBrokerQueues.MediaInfoUpdatedQueue);
    }

    private async Task PublishMessageAsync<T>(T message, string queueName)
        where T : class
    {
        var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));

        await sendEndPoint.Send(message);
    }
}
