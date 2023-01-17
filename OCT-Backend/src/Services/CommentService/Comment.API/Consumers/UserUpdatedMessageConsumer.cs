using AutoMapper;
using Comment.Contract.Services;
using Comment.Domain;
using MassTransit;
using Messages.UserService;
using Serilog;

namespace Comment.API.Consumers;

public class UserUpdatedMessageConsumer : IConsumer<UserUpdatedMessage>
{
    private readonly IUserService _service;
    private readonly IMapper _mapper;

    public UserUpdatedMessageConsumer(IUserService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<UserUpdatedMessage> context)
    {
        Log.Debug("{name} message was received: {@message}", nameof(UserUpdatedMessage), context.Message);

        var userModel = _mapper.Map<UserModel>(context.Message);

        await _service.UpdateAsync(context.Message.UserId, userModel);
    }
}
