using EasySchedule.Domain.Entities;
using FluentValidation;

namespace EasySchedule.Application.Validators;

public class ScheduleValidator : AbstractValidator<Schedule>
{
    public ScheduleValidator()
    {
        RuleFor(s => s.Name).NotEmpty().MaximumLength(100);
        RuleFor(s => s.EndDate)
            .GreaterThanOrEqualTo(s => s.StartDate)
            .WithMessage("Data zakończenia musi być późniejsza lub równa dacie rozpoczęcia.");
    }
}