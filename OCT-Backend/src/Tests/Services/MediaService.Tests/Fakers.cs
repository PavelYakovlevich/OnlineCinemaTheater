using Bogus;
using Media.Domain;
using Microsoft.AspNetCore.Http;
using Moq;

namespace MediaService.Tests;

internal static class Fakers
{
    public static readonly Faker<MediaInfoModel> MediaInfoModelFaker = new Faker<MediaInfoModel>()
        .RuleFor(_ => _.Id, _ => Guid.NewGuid())
        .RuleFor(_ => _.IsTvSerias, f => f.Random.Bool())
        .RuleFor(_ => _.IsVisible, f => f.Random.Bool());

    public static readonly Faker<MediaContentModel> MediaContentModelFaker = new Faker<MediaContentModel>()
        .RuleFor(_ => _.Id, _ => Guid.NewGuid())
        .RuleFor(_ => _.MediaId, _ => Guid.NewGuid())
        .RuleFor(_ => _.IssueDate, _ => DateTime.UtcNow)
        .RuleFor(_ => _.Number, f => f.Random.Number(100))
        .RuleFor(_ => _.ContentFile, _ => new Mock<IFormFile>().Object);
}
