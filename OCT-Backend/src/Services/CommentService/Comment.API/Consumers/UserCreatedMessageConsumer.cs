using Comment.Contract.Services;
using MassTransit;
using Messages.AuthenticationService;
using Messages.UserService;
using Serilog;

namespace Comment.API.Consumers;

public class UserCreatedMessageConsumer : IConsumer<UserCreatedMessage>
{
    private readonly IUserService _service;

    public UserCreatedMessageConsumer(IUserService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<UserCreatedMessage> context)
    {
        Log.Debug("{name} message was received: {@message}", nameof(UserCreatedMessage), context.Message);

        await _service.CreateAsync(context.Message.Id);
    }
}
