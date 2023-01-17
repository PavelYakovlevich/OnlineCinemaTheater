using FluentValidation;
using Models.MediaInfoService.Request.Participant;

namespace MediaInfo.API.Validators.Participant;

public class ParticipantsRequestFiltersValidator : AbstractValidator<ParticipantsRequestFilters>
{
	public ParticipantsRequestFiltersValidator()
	{
		When(_ => _.Role is not null, () =>
		{
			RuleFor(_ => _.Role).IsInEnum();
		});

        RuleFor(_ => _.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset must be a positive number");

        RuleFor(_ => _.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0");
    }
}
