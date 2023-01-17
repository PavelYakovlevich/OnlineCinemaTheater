using Bogus;
using Models.UserService.Requests;
using User.Domain.Models;

namespace UserService.Tests;

internal static class Fakers
{
    public static readonly Faker<UserModel> UserModelFaker = new Faker<UserModel>()
        .RuleFor(p => p.Id, f => Guid.NewGuid())
        .RuleFor(p => p.Name, f => f.Name.FirstName())
        .RuleFor(p => p.Surname, f => f.Name.LastName())
        .RuleFor(p => p.MiddleName, f => f.Name.FirstName())
        .RuleFor(p => p.Birthday, f => f.Date.Past())
        .RuleFor(p => p.PhotoLink, f => f.Image.LoremPixelUrl())
        .RuleFor(p => p.Description, f => f.Random.Words());

    public static readonly Faker<ChangeUserRequest> ChangeRequestFaker = new Faker<ChangeUserRequest>()
        .RuleFor(p => p.Name, f => f.Name.FirstName())
        .RuleFor(p => p.Surname, f => f.Name.LastName())
        .RuleFor(p => p.MiddleName, f => f.Name.FirstName())
        .RuleFor(p => p.Birthday, f => f.Date.Past())
        .RuleFor(p => p.Description, f => f.Random.Words());
}
