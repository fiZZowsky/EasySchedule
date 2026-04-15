using EasySchedule.Domain.Entities;
using FluentValidation;

namespace EasySchedule.Application.Validators;

public class EmployeeValidator : AbstractValidator<Employee>
{
    public EmployeeValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Imię pracownika jest wymagane.")
            .MaximumLength(50).WithMessage("Imię jest zbyt długie.");

        RuleFor(e => e.Surname)
            .NotEmpty().WithMessage("Nazwisko pracownika jest wymagane.")
            .MaximumLength(50).WithMessage("Nazwisko jest zbyt długie.");

        RuleFor(e => e.PhoneNumber).Matches(@"^\d{9}$").WithMessage("Podaj poprawny numer.");
    }
}