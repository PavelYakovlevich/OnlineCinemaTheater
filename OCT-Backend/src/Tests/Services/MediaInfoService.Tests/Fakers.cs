using Bogus;
using Enums.MediaInfoService;
using MediaInfo.Domain.Genre;
using MediaInfo.Domain.MediaInfo;
using MediaInfo.Domain.Participant;

namespace MediaInfoService.Tests;

internal class Fakers
{
    public static readonly Faker<GenreModel> GenreModelFaker = new Faker<GenreModel>()
        .RuleFor(_ => _.Id, _ => Guid.NewGuid())
        .RuleFor(_ => _.Name, f => f.Random.Word());

    public static readonly Faker<ParticipantModel> ParticipantModelFaker = new Faker<ParticipantModel>()
        .RuleFor(_ => _.Id, _ => Guid.NewGuid())
        .RuleFor(_ => _.Name, f => f.Person.FirstName)
        .RuleFor(_ => _.Surname, f => f.Person.LastName)
        .RuleFor(_ => _.Birthday, _ => DateTime.UtcNow)
        .RuleFor(_ => _.Description, f => f.Random.Words())
        .RuleFor(_ => _.Role, f => f.Random.Enum<ParticipantRole>())
        .RuleFor(_ => _.Country, f => new string(f.Random.Chars('a', 'z', count: 3)))
        .RuleFor(_ => _.Picture, f => f.Person.Avatar);

    public static readonly Faker<MediaInfoModel> MediaInfoModelFaker = new Faker<MediaInfoModel>()
        .RuleFor(_ => _.Id, _ => Guid.NewGuid())
        .RuleFor(_ => _.Name, f => new string(f.Random.Chars('a', 'z', count: 3)))
        .RuleFor(_ => _.IssueDate, _ => DateTime.UtcNow)
        .RuleFor(_ => _.IsTvSerias, f => f.Random.Bool())
        .RuleFor(_ => _.IsFree, f => f.Random.Bool())
        .RuleFor(_ => _.Description, f => f.Random.Words())
        .RuleFor(_ => _.AgeRating, f => f.Random.Enum<AgeRating>())
        .RuleFor(_ => _.Country, f => new string(f.Random.Chars('a', 'z', count: 3)))
        .RuleFor(_ => _.Budget, f => f.Random.Decimal())
        .RuleFor(_ => _.Aid, f => f.Random.Decimal())
        .RuleFor(_ => _.Genres, f => GenreModelFaker.Generate(f.Random.Number(1, 10)))
        .RuleFor(_ => _.Participants, f => ParticipantModelFaker.Generate(f.Random.Number(1, 10)));

    public static readonly Faker<MediaInfoFiltersModel> MediaInfoFiltersModelFaker = new Faker<MediaInfoFiltersModel>()
        .RuleFor(_ => _.Country, f => new string(f.Random.Chars('a', 'z', count: 3)))
        .RuleFor(_ => _.Genres, f => Enumerable.Repeat(new string(f.Random.Chars('a', 'z', count: 3)), f.Random.Number(1, 10)))
        .RuleFor(_ => _.NameStartsWith, f => new string(f.Random.Chars('a', 'z', count: 3)))
        .RuleFor(_ => _.IsTvSerias, f => f.Random.Bool())
        .RuleFor(_ => _.Limit, f => f.Random.Number(1, 10))
        .RuleFor(_ => _.Offset, f => f.Random.Number(0, 10));

    public static readonly Faker<PartialUpdateMediaInfoModel> PartialUpdateMediaInfoModelFaker = new Faker<PartialUpdateMediaInfoModel>()
        .RuleFor(_ => _.Name, f => new string(f.Random.Chars('a', 'z', count: 3)))
        .RuleFor(_ => _.IssueDate, _ => DateTime.UtcNow)
        .RuleFor(_ => _.IsTvSerias, f => f.Random.Bool())
        .RuleFor(_ => _.IsFree, f => f.Random.Bool())
        .RuleFor(_ => _.Description, f => f.Random.Words())
        .RuleFor(_ => _.AgeRating, f => f.Random.Enum<AgeRating>())
        .RuleFor(_ => _.Country, f => new string(f.Random.Chars('a', 'z', count: 3)))
        .RuleFor(_ => _.Budget, f => f.Random.Decimal())
        .RuleFor(_ => _.Aid, f => f.Random.Decimal())
        .RuleFor(_ => _.Genres, f => Enumerable.Repeat(f.Random.Uuid(), f.Random.Number(1, 10)).ToArray())
        .RuleFor(_ => _.Participants, f => Enumerable.Repeat(f.Random.Uuid(), f.Random.Number(1, 10)).ToArray());
}