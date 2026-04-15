using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;

namespace EasySchedule.Application.Interfaces.Rules;

public interface IScheduleRule
{
    string RuleName { get; }
    RuleSeverity Severity { get; }

    bool IsValid(
        ShiftAssignment proposedAssignment,
        IEnumerable<ShiftAssignment> currentAssignments,
        ScheduleSettings settings,
        IEnumerable<TimeOff> timeOffs);
}