using EasySchedule.Domain.Entities;
using FluentValidation;

namespace EasySchedule.Application.Validators;

public class TimeOffValidator : AbstractValidator<TimeOff>
{
    public TimeOffValidator()
    {
        RuleFor(t => t.EmployeeId)
            .GreaterThan(0).WithMessage("Wymagane jest przypisanie pracownika.");

        RuleFor(t => t.EndDate)
            .GreaterThanOrEqualTo(t => t.StartDate)
            .WithMessage("Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.");

        RuleFor(t => t.Comments)
            .MaximumLength(250).WithMessage("Komentarz nie może przekraczać 250 znaków.");
    }
}