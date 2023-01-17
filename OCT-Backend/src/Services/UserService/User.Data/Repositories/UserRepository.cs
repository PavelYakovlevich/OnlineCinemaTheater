using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using User.Contract.Repositories;
using User.Domain.Models;
using static Dapper.SqlMapper;
using UserEntity = User.Data.Entities.User;

namespace User.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;

    public UserRepository(string connectionString, IMapper mapper)
    {
        _connectionString = connectionString;
        _mapper = mapper;
    }

    public async Task CreateUserAsync(UserModel userModel)
    {
        await using var connection = new SqlConnection(_connectionString);

        var entity = _mapper.Map<UserEntity>(userModel);

        connection.Query(@"
            INSERT INTO Users
            (Id, Name, MiddleName, Surname, Birthday, Photo, Description)
            VALUES 
            (@id, @name, @middleName, @surname, @birthday, @photo, @description)",
            entity);
    }

    public async Task<UserModel> GetUserById(Guid id)
    {
        await using var connection = new SqlConnection(_connectionString);

        var entity = await connection.QueryFirstOrDefaultAsync<UserEntity>($@"
            SELECT * FROM Users WHERE Id = '{id}'");

        return _mapper.Map<UserModel>(entity);
    }

    public async Task<bool> UpdateUserAsync(Guid id, UserModel userModel)
    {
        await using var connection = new SqlConnection(_connectionString);

        var entity = _mapper.Map<UserEntity>(userModel);

        var rowsAffectedCount = await connection.ExecuteAsync($@"
            UPDATE Users SET
                Name        = @name,
                Surname     = @surname,
                Middlename  = @middlename,
                Birthday    = @birthday,
                Photo       = @photo,
                Description = @description
            WHERE Id = '{id}'", entity);

        return rowsAffectedCount != 0;
    }
}
