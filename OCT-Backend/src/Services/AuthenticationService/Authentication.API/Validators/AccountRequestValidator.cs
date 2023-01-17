﻿using FluentValidation;
using Models.AuthenticationService.Request;

namespace Authentication.API.Validators;

public class AccountRequestValidator : AbstractValidator<AccountRequest>
{
    public AccountRequestValidator()
    {
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(model => model.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password length must be greater than 8 symbols.")
            .MaximumLength(50).WithMessage("Password length must be less than 50 symbols.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase character.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase character.")
            .Matches("^[\\w]+$").WithMessage("Password can't contain special characters.");
    }
}
