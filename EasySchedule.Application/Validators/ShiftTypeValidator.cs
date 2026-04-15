using EasySchedule.Domain.Entities;
using FluentValidation;

namespace EasySchedule.Application.Validators;

public class ShiftTypeValidator : AbstractValidator<ShiftType>
{
    public ShiftTypeValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty().WithMessage("Nazwa zmiany jest wymagana.")
            .MaximumLength(50).WithMessage("Nazwa jest zbyt długa.");

        RuleFor(s => s.ShortName)
            .NotEmpty().WithMessage("Skrót zmiany jest wymagany (np. 'D', 'N').")
            .MaximumLength(5).WithMessage("Skrót może mieć maksymalnie 5 znaków.");

        RuleFor(s => s)
            .Must(s => s.StartTime != s.EndTime)
            .WithMessage("Godzina rozpoczęcia i zakończenia nie mogą być takie same.");
    }
}