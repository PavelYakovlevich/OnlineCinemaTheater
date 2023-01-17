using MassTransit;
using Messages.AuthenticationService;
using Serilog;
using User.Contract.Services;

namespace User.API.Consumers;

public class CreateUserMessageConsumer : IConsumer<CreateUserMessage>
{
    private readonly IUserService _userService;

    public CreateUserMessageConsumer(IUserService userService)
    {
        _userService = userService;
    }

    public async Task Consume(ConsumeContext<CreateUserMessage> context)
    {
        Log.Debug("{messageName} message was received: {@message}", 
            nameof(CreateUserMessage),
            context.Message);

        await _userService.CreateUserAsync(context.Message.AccountId);
    }
}
