using AutoMapper;
using Comment.Contract.Repositories;
using Comment.Data.Entities;
using Comment.Domain;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Comment.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;
    private readonly IMongoCollection<User> _users;

    public UserRepository(string connectionString, IMapper mapper)
    {
        _connectionString = connectionString;
        _mapper = mapper;
        
        var database = new MongoClient(_connectionString).GetDatabase("comment");

        _users = database.GetCollection<User>("users");
    }

    public async Task CreateAsync(Guid id, CancellationToken token = default)
    {
        var user = new User { Id = id };

        await _users.InsertOneAsync(user, new InsertOneOptions(), token);
    }

    public async Task<UserModel> ReadByIdAsync(Guid id, CancellationToken token = default)
    {
        var users = await _users.FindAsync(new BsonDocument("_id", id.ToString()), cancellationToken: token);

        var user = users.FirstOrDefault(token);

        return _mapper.Map<UserModel>(user);
    }

    public async Task<bool> UpdateAsync(Guid id, UserModel userModel, CancellationToken token = default)
    {
        var user = _mapper.Map<User>(userModel);

        var result = await _users.ReplaceOneAsync(new BsonDocument("_id", id.ToString()), user, cancellationToken: token);

        return result.ModifiedCount == 1;
    }
}
