using EasySchedule.Domain.Entities;
using FluentValidation;

namespace EasySchedule.Application.Validators;

public class ProfessionValidator : AbstractValidator<Profession>
{
    public ProfessionValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Nazwa zawodu jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa zawodu nie może przekraczać 100 znaków.");
    }
}