using AutoMapper;
using MassTransit;
using Media.Contract.Services;
using Media.Domain;
using Messages.MediaInfoService;
using Serilog;

namespace Media.API.Consumers;

public class MediaInfoUpdatedMessageConsumer : IConsumer<MediaInfoUpdatedMessage>
{
    private readonly IMediaService _service;
    private readonly IMapper _mapper;

    public MediaInfoUpdatedMessageConsumer(IMediaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<MediaInfoUpdatedMessage> context)
    {
        Log.Debug("{name} message was received: {@message}", nameof(MediaInfoUpdatedMessage), context.Message);

        var mediaInfoModel = _mapper.Map<MediaInfoModel>(context.Message);

        await _service.UpdateMediaAsync(context.Message.Id, mediaInfoModel);
    }
}
