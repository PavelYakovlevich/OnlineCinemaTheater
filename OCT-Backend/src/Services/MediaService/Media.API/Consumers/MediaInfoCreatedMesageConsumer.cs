using AutoMapper;
using MassTransit;
using Media.Contract.Services;
using Media.Domain;
using Messages.MediaInfoService;
using Serilog;

namespace Media.API.Consumers;

public class MediaInfoCreatedMesageConsumer : IConsumer<MediaInfoCreatedMessage>
{
    private readonly IMediaService _service;
    private readonly IMapper _mapper;

    public MediaInfoCreatedMesageConsumer(IMediaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<MediaInfoCreatedMessage> context)
    {
        Log.Debug("{name} message was received: {@message}", nameof(MediaInfoCreatedMessage), context.Message);

        var mediaInfoModel = _mapper.Map<MediaInfoModel>(context.Message);

        await _service.CreateMediaAsync(mediaInfoModel);
    }
}
