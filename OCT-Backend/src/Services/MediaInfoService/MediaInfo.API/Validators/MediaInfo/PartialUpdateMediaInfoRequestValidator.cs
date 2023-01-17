using FluentValidation;
using MediaInfo.Domain.MediaInfo;

namespace MediaInfo.API.Validators.MediaInfo
{
    public class PartialUpdateMediaInfoValidator : AbstractValidator<PartialUpdateMediaInfoModel>
    {
        public PartialUpdateMediaInfoValidator()
        {
            When(_ => _.Name is not null, () =>
            {
                RuleFor(_ => _.Name)
                    .NotEmpty().WithMessage("Name is empty")
                    .MaximumLength(50).WithMessage("Name is longer than 50 characters");
            });

            When(_ => _.Budget is not null, () =>
            {
                RuleFor(_ => _.Budget).GreaterThan(0);
            });

            When(_ => _.Aid is not null, () =>
            {
                RuleFor(_ => _.Aid).GreaterThan(0);
            });

            RuleFor(_ => _.AgeRating).IsInEnum();

            When(_ => _.Country is not null, () =>
            {
                RuleFor(_ => _.Country)
                    .NotEmpty().WithMessage("Country code is empty")
                    .Length(3).WithMessage("Country code must be 3 characters long");
            });

            When(_ => _.Participants is not null, () =>
            {
                RuleFor(_ => _.Participants)
                    .NotEmpty().WithMessage("Participants list is empty")
                    .Must(c => c.Distinct().Count() == c.Count()).WithMessage("All participants ids must be unique");
            });

            When(_ => _.Genres is not null, () =>
            {
                RuleFor(_ => _.Genres)
                    .NotEmpty().WithMessage("Genres list is empty")
                    .Must(c => c.Distinct().Count() == c.Count()).WithMessage("All genres ids must be unique");
            });
        }
    }
}
