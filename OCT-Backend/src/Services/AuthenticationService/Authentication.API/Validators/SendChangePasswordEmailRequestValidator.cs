using FluentValidation;
using Models.AuthenticationService.Request;

namespace Authentication.API.Validators;

public class SendChangePasswordEmailRequestValidator : AbstractValidator<SendChangePasswordEmailRequest>
{
    public SendChangePasswordEmailRequestValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
