using FluentValidation;
using Models.MediaInfoService.Request.MediaInfo;

namespace MediaInfo.API.Validators.MediaInfo;

public class MediaInfoRequestValidator : AbstractValidator<MediaInfoRequest>
{
    public MediaInfoRequestValidator()
    {
        RuleFor(_ => _.Name)
            .NotEmpty().WithMessage("Name is empty")
            .MaximumLength(50).WithMessage("Name is longer than 50 characters");

        When(_ => _.Budget is not null, () =>
        {
            RuleFor(_ => _.Budget).GreaterThan(0);
        });

        When(_ => _.Aid is not null, () =>
        {
            RuleFor(_ => _.Aid).GreaterThan(0);
        });

        RuleFor(_ => _.AgeRating).IsInEnum();

        RuleFor(_ => _.Country)
            .NotEmpty().WithMessage("Country code is empty")
            .Length(3).WithMessage("Country code must be 3 characters long");

        RuleFor(_ => _.Participants)
            .NotEmpty().WithMessage("Participants list is empty")
            .Must(c => c.Distinct().Count() == c.Count()).WithMessage("All participants ids must be unique");

        RuleFor(_ => _.Genres)
            .NotEmpty().WithMessage("Genres list is empty")
            .Must(c => c.Distinct().Count() == c.Count()).WithMessage("All genres ids must be unique");
    }
}
