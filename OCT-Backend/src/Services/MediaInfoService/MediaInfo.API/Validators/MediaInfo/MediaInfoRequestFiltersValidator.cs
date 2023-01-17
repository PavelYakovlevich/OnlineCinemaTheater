using FluentValidation;
using Models.MediaInfoService.Request.MediaInfo;

namespace MediaInfo.API.Validators.MediaInfo;

public class MediaInfoRequestFiltersValidator : AbstractValidator<MediaInfoRequestFilters>
{
    public MediaInfoRequestFiltersValidator()
    {
        RuleFor(_ => _.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset must be a positive number");

        RuleFor(_ => _.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0");

        When(_ => _.Genres is not null, () =>
        {
            RuleFor(_ => _.Genres)
                .NotEmpty().WithMessage("Genres list is empty")
                .Must(c => c.Distinct().Count() == c.Length).WithMessage("All genres ids must be unique");
        });

        When(_ => _.Country is not null, () =>
        {
            RuleFor(_ => _.Country)
                .NotEmpty().WithMessage("Country code is empty")
                .Length(3).WithMessage("Country code must be 3 characters long");
        });
    }
}
