namespace Infrastructure.MessageBroker.Abstractions;

public interface IMediaInfoPublishService
{
    Task PublishMediaInfoCreatedMessage(Guid id, bool isTvSerias, bool isVisible);

    Task PublishMediaInfoDeletedMessage(Guid id);

    Task PublishMediaInfoUpdatedMessage(Guid id, bool isTvSerias, bool isVisible);
}
