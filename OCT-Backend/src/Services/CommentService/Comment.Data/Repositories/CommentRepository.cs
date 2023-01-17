using AutoMapper;
using Comment.Contract.Repositories;
using Comment.Domain;
using MongoDB.Driver;
using CommentEntity = Comment.Data.Entities.Comment;

namespace Comment.Data.Repositories;

public class CommentRepository : ICommentRepository
{
	private readonly string _connectionString;
	private readonly IMapper _mapper;
	private readonly IMongoCollection<CommentEntity> _comments;

	public CommentRepository(string connectionString, IMapper mapper)
	{
		_connectionString = connectionString;
		_mapper = mapper;

        var database = new MongoClient(_connectionString).GetDatabase("comment");

        _comments = database.GetCollection<CommentEntity>("comments");
    }

	public async Task<Guid> CreateAsync(CommentModel commentModel, CancellationToken token = default)
	{
		var comment = _mapper.Map<CommentEntity>(commentModel);
		comment.Id = Guid.NewGuid();

		await _comments.InsertOneAsync(comment, cancellationToken: token);

		return comment.Id;
	}

	public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
	{
		var result = await _comments.DeleteOneAsync(c => c.Id == id, token);

		return result.DeletedCount == 1;
	}

	public IAsyncEnumerable<CommentModel> ReadAllAsync(Guid mediaId, CommentsFilters filters)
	{
		return _comments
			.Aggregate()
			.Lookup("users", "UserId", "_id", "User")
			.Unwind("User")
			.As<CommentEntity>()
			.Match(c => c.MediaId == mediaId)
			.SortByDescending(c => c.IssueDate)
			.Skip(filters.Offset)
			.Limit(filters.Limit)
			.ToEnumerable()
			.ToAsyncEnumerable()
			.Select(c => _mapper.Map<CommentModel>(c));
	}

	public async Task<bool> UpdateAsync(Guid id, CommentModel commentModel, CancellationToken token = default)
	{
        var comment = _mapper.Map<CommentEntity>(commentModel);
		comment.Id = id;

		var updateDefinition = CreateUpdateDefinition(commentModel);

        var result = await _comments.UpdateOneAsync(c => c.Id == id, updateDefinition, cancellationToken: token);

        return result.ModifiedCount == 1;
    }

    public async Task<CommentModel> ReadByIdAsync(Guid id, CancellationToken token = default)
    {
		var comment = await _comments
			.Aggregate()
			.Match(c => c.Id == id)
            .Lookup("users", "UserId", "_id", "User")
			.Unwind("User")
			.As<CommentEntity>()
			.FirstOrDefaultAsync(token);

		return _mapper.Map<CommentModel>(comment);
    }

    private static UpdateDefinition<CommentEntity> CreateUpdateDefinition(CommentModel commentModel)
	{
		var update = Builders<CommentEntity>.Update;

		return update.Set(c => c.Text, commentModel.Text);
	}
}
