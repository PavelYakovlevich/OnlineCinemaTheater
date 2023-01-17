using FluentValidation;
using Models.UserService.Requests;

namespace User.API.Validators;

public class ChangeUserRequestValidator : AbstractValidator<ChangeUserRequest>
{
	public ChangeUserRequestValidator()
	{
		RuleFor(cr => cr.Name)
			.NotEmpty().WithMessage("Name is empty")
			.MaximumLength(50).WithMessage("Name length must be less than 50")
			.Matches("^[a-zA-Z]+$").WithMessage("Name can contain only letters");

        RuleFor(cr => cr.Surname)
			.NotEmpty().WithMessage("Surname is empty")
            .MaximumLength(50).WithMessage("Surname length must be less than 50")
            .Matches("^[a-zA-Z]+$").WithMessage("Surname can contain only letters");

		When(cr => cr.MiddleName is not null, () =>
        {
			RuleFor(cr => cr.MiddleName)
				.NotEmpty().WithMessage("Middlename is empty")
                .MaximumLength(50)
				.WithMessage("Middlename length must be less than 50")
				.Matches("^[a-zA-Z]*$")
				.WithMessage("Middlename can contain only letters");
        });

		RuleFor(cr => cr.Birthday)
			.NotEmpty().WithMessage("Date of birth is empty")
			.LessThan(DateTime.UtcNow).WithMessage($"Date of birth must be less than {DateTime.UtcNow.ToShortDateString()}");
    }
}
