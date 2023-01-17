using AutoMapper;
using Media.Contract.Repositories;
using Media.Data.Context;
using Media.Data.Entities;
using Media.Domain;
using Microsoft.EntityFrameworkCore;

namespace Media.Data.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly MediaServiceDbContext _context;
    private readonly IMapper _mapper;

    public MediaRepository(MediaServiceDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Guid> CreateAsync(MediaContentModel mediaInfoModel, CancellationToken token = default)
    {
        var mediaInfo = _mapper.Map<MediaContent>(mediaInfoModel);

        var entityEntry = await _context.AddAsync(mediaInfo, token);

        await _context.SaveChangesAsync(token);

        return entityEntry.Entity.Id;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        var mediaContent = await _context.MediaContents.FirstOrDefaultAsync(c => c.Id == id, token);

        _context.Remove(mediaContent);

        return await _context.SaveChangesAsync(token) == 1;
    }

    public IAsyncEnumerable<MediaContentModel> ReadAllByMediaIdAsync(Guid mediaId, CancellationToken token = default)
    {
        if (_context.MediaInfos.All(m => m.Id != mediaId))
        {
            return null;
        }

        return _context.MediaContents
            .Where(c => c.MediaId == mediaId)
            .OrderBy(c => c.Number)
            .AsNoTracking()
            .Select(c => _mapper.Map<MediaContentModel>(c))
            .AsAsyncEnumerable();
    }

    public async Task<MediaContentModel> ReadByIdAsync(Guid id, CancellationToken token = default)
    {
        var mediaContent = await _context.MediaContents
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, token);

        return _mapper.Map<MediaContentModel>(mediaContent);
    }
}
