namespace Infrastructure.MessageBroker.Abstractions;

public interface IUserPublishService
{
    Task PublishUserCreatedMessage(Guid id);

    Task PublishUserUpdatedMessage(Guid id, string name, string surname);
}
