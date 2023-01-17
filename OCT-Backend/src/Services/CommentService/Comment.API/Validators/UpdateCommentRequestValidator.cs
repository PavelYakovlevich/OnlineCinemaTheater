using FluentValidation;
using Models.CommentService.Requests;

namespace Comment.API.Validators;

public class UpdateCommentRequestValidator : AbstractValidator<UpdateCommentRequest>
{
	public UpdateCommentRequestValidator()
	{
        RuleFor(_ => _.Text).NotEmpty().WithMessage("Text is empty");
    }
}

