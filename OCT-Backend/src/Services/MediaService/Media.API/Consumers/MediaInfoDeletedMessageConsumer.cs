using MassTransit;
using Media.Contract.Services;
using Messages.MediaInfoService;
using Serilog;

namespace Media.API.Consumers;

public class MediaInfoDeletedMessageConsumer : IConsumer<MediaInfoDeletedMessage>
{
    private readonly IMediaService _service;

    public MediaInfoDeletedMessageConsumer(IMediaService service)
    {
        _service = service;
    }

    public async Task Consume(ConsumeContext<MediaInfoDeletedMessage> context)
    {
        Log.Debug("{name} message was received: {@message}", nameof(MediaInfoDeletedMessage), context.Message);

        await _service.DeleteMediaAsync(context.Message.Id);
    }
}
