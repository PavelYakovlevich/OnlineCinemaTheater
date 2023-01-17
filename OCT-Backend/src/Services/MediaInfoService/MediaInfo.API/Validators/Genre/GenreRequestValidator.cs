using FluentValidation;
using Models.MediaInfoService.Request.Genre;

namespace MediaInfo.API.Validators.Genre;

public class GenreRequestValidator : AbstractValidator<GenreRequest>
{
    public GenreRequestValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Name is empty")
            .MaximumLength(50).WithMessage("Name length is longer than 50 chars");
    }
}
