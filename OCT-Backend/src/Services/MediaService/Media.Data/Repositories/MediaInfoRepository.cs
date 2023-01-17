using AutoMapper;
using Media.Contract.Repositories;
using Media.Data.Context;
using Media.Data.Entities;
using Media.Domain;
using Microsoft.EntityFrameworkCore;

namespace Media.Data.Repositories;

public class MediaInfoRepository : IMediaInfoRepository
{
    private readonly MediaServiceDbContext _context;
    private readonly IMapper _mapper;

    public MediaInfoRepository(MediaServiceDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Guid> CreateMediaInfoAsync(MediaInfoModel mediaInfoModel, CancellationToken token = default)
    {
        var mediaInfo = _mapper.Map<MediaInfo>(mediaInfoModel);

        var entityEntry = await _context.MediaInfos.AddAsync(mediaInfo, token);

        await _context.SaveChangesAsync(token);
        
        return entityEntry.Entity.Id;
    }

    public async Task<bool> DeleteMediaInfoAsync(Guid id, CancellationToken token = default)
    {
        var mediaInfo = await _context.MediaInfos.FirstOrDefaultAsync(m => m.Id == id, token);

        if (mediaInfo is null)
        {
            return false;
        }

        _context.Remove(mediaInfo);

        return await _context.SaveChangesAsync(token) == 1;
    }

    public async Task<bool> UpdateMediaInfoAsync(Guid id, MediaInfoModel mediaInfoModel, CancellationToken token = default)
    {
        var mediaInfo = await _context.MediaInfos.FirstOrDefaultAsync(m => m.Id == id, token);

        if (mediaInfo is null)
        {
            return false;
        }

        _mapper.Map(mediaInfoModel, mediaInfo);

        return await _context.SaveChangesAsync(token) == 1;
    }

    public async Task<MediaInfoModel> ReadMediaInfoAsync(Guid id, CancellationToken token = default)
    {
        var mediaInfo = await _context.MediaInfos.FirstOrDefaultAsync(m => m.Id == id, token);

        return _mapper.Map<MediaInfoModel>(mediaInfo);
    }
}
