using Bogus;
using Comment.API.Validators;
using Comment.Domain;
using Models.CommentService.Requests;

namespace CommentService.Tests;

internal static class Fakers
{
    public static readonly Faker<CreateCommentRequest> CreateCommentRequestFaker = new Faker<CreateCommentRequest>()
        .RuleFor(_ => _.UserId, f => Guid.NewGuid())
        .RuleFor(_ => _.Text, f => f.Random.Words());

    public static readonly Faker<GetCommentsFilters> GetCommentsFiltersFaker = new Faker<GetCommentsFilters>()
        .RuleFor(_ => _.Offset, f => f.Random.Number(0, int.MaxValue))
        .RuleFor(_ => _.Limit, f => f.Random.Number(1, int.MaxValue));

    public static readonly Faker<UpdateCommentRequest> UpdateCommentRequestFaker = new Faker<UpdateCommentRequest>()
        .RuleFor(_ => _.UserId, f => Guid.NewGuid())
        .RuleFor(_ => _.Text, f => f.Random.Words());

    public static readonly Faker<CommentModel> CommentModelFaker = new Faker<CommentModel>()
        .RuleFor(_ => _.Id, f => Guid.NewGuid())
        .RuleFor(_ => _.UserId, f => Guid.NewGuid())
        .RuleFor(_ => _.IssueDate, f => DateTime.UtcNow)
        .RuleFor(_ => _.Text, f => f.Random.Words());

    public static readonly Faker<UserModel> UserModelFaker = new Faker<UserModel>()
        .RuleFor(_ => _.Id, f => Guid.NewGuid())
        .RuleFor(_ => _.Name, f => f.Person.FirstName)
        .RuleFor(_ => _.Surname, f => f.Person.LastName);
}
