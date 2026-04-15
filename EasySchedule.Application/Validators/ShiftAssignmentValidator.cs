using EasySchedule.Domain.Entities;
using FluentValidation;

namespace EasySchedule.Application.Validators;

public class ShiftAssignmentValidator : AbstractValidator<ShiftAssignment>
{
    public ShiftAssignmentValidator()
    {
        RuleFor(sa => sa.ScheduleId).GreaterThan(0);
        RuleFor(sa => sa.EmployeeId).GreaterThan(0);
        RuleFor(sa => sa.ShiftTypeId).GreaterThan(0);
    }
}