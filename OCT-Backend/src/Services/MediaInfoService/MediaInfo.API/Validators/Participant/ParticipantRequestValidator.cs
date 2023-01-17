using Enums.MediaInfoService;
using FluentValidation;
using Models.MediaInfoService.Request.Participant;

namespace MediaInfo.API.Validators.Participant;

public class ParticipantRequestValidator : AbstractValidator<ParticipantRequest>
{
    public ParticipantRequestValidator()
    {
        RuleFor(cr => cr.Name)
            .NotEmpty().WithMessage("Name is empty")
            .MaximumLength(50).WithMessage("Name length must be less than 50")
            .Matches("^[a-zA-Z]+$").WithMessage("Name can contain only letters");

        RuleFor(cr => cr.Surname)
            .NotEmpty().WithMessage("Surname is empty")
            .MaximumLength(50).WithMessage("Surname length must be less than 50")
            .Matches("^[a-zA-Z]+$").WithMessage("Surname can contain only letters");

        When(cr => cr is not null, () =>
        {
            RuleFor(cr => cr.Birthday)
                .NotEmpty().WithMessage("Date of birth is empty")
                .LessThan(DateTime.UtcNow).WithMessage($"Date of birth must be less than {DateTime.UtcNow.ToShortDateString()}");
        });

        RuleFor(cr => cr.Role)
            .IsInEnum().WithMessage("Invalid");

        RuleFor(cr => cr.Country)
            .NotEmpty().WithMessage("Country code is empty")
            .Length(3).WithMessage("Country code must be 3 characters long");
    }
}
