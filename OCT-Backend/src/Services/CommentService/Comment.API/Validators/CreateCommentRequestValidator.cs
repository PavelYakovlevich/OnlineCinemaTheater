using FluentValidation;
using Models.CommentService.Requests;

namespace Comment.API.Validators;

public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
{
    public CreateCommentRequestValidator()
    {
        RuleFor(_ => _.Text).NotEmpty().WithMessage("Text is empty");
    }
}
