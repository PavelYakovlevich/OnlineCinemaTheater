using Comment.Domain;

namespace Comment.Contract.Repositories;

public interface ICommentRepository
{
    Task<Guid> CreateAsync(CommentModel commentModel, CancellationToken token = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken token = default);

    IAsyncEnumerable<CommentModel> ReadAllAsync(Guid mediaId, CommentsFilters filters);

    Task<CommentModel> ReadByIdAsync(Guid id, CancellationToken token = default);

    Task<bool> UpdateAsync(Guid id, CommentModel commentModel, CancellationToken token = default);
}
