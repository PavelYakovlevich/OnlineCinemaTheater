using Comment.Contract.Repositories;
using Comment.Contract.Services;
using Comment.Domain;
using Exceptions;
using Serilog;

namespace Comment.Core;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUserRepository _userRepository;

    public CommentService(ICommentRepository commentRepository, IUserRepository userRepository)
    {
        _commentRepository = commentRepository;
        _userRepository = userRepository;
    }

    public async Task<Guid> CreateAsync(CommentModel commentModel)
    {
        if (await _userRepository.ReadByIdAsync(commentModel.UserId) is null)
        {
            throw new ResourceNotFoundException($"User {commentModel.UserId} was not found");
        }

        commentModel.IssueDate = DateTime.UtcNow;

        var id = await _commentRepository.CreateAsync(commentModel);

        Log.Debug("Comment {id} was created: {@comment}", id, commentModel);

        return id;
    }

    public async Task DeleteAsync(Guid id)
    {
        if (!await _commentRepository.DeleteAsync(id))
        {
            throw new ResourceNotFoundException($"Comment {id} was not found");
        }

        Log.Debug("Comment {id} was deleted", id);
    }

    public async Task<CommentModel> GetByIdAsync(Guid id)
    {
        var comment = await _commentRepository.ReadByIdAsync(id) ??
            throw new ResourceNotFoundException($"Comment {id} was not found");

        Log.Debug("Comment {id} was found: {@model}", id, comment);

        return comment;
    }

    public async IAsyncEnumerable<CommentModel> GetCommentsAsync(Guid mediaId, CommentsFilters filters)
    {
        var comments = _commentRepository.ReadAllAsync(mediaId, filters) ??
            throw new ResourceNotFoundException($"Media {mediaId} was not found");

        int count = 0;
        await foreach (var comment in comments)
        {
            yield return comment;
            count++;
        }

        Log.Debug("{count} comments were found for media {id}: {@filters}", count, mediaId, filters);
    }

    public async Task UpdateAsync(Guid id, CommentModel commentModel)
    {
        if (!await _commentRepository.UpdateAsync(id, commentModel))
        {
            throw new ResourceNotFoundException($"Comment {id} was not found");
        }

        Log.Debug("Comment {id} was updated with: {@model}", id, commentModel);
    }
}
