using Comment.Domain;

namespace Comment.Contract.Services;

public interface ICommentService
{
    Task<Guid> CreateAsync(CommentModel commentModel);

    Task DeleteAsync(Guid id);

    IAsyncEnumerable<CommentModel> GetCommentsAsync(Guid mediaId, CommentsFilters filters);

    Task<CommentModel> GetByIdAsync(Guid id);

    Task UpdateAsync(Guid id, CommentModel commentModel);
}
