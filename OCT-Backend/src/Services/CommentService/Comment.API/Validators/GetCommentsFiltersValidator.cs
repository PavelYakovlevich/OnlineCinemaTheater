using FluentValidation;
using Models.CommentService.Requests;

namespace Comment.API.Validators;

public class GetCommentsFiltersValidator : AbstractValidator<GetCommentsFilters>
{
	public GetCommentsFiltersValidator()
	{
        RuleFor(_ => _.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset must be a positive number");

        RuleFor(_ => _.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0");
    }
}
