using EasySchedule.Application.Interfaces.Rules;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;

namespace EasySchedule.Application.Rules;

public class TimeOffRule : IScheduleRule
{
    public string RuleName => "Brak nakładających się urlopów";
    public RuleSeverity Severity => RuleSeverity.Hard;

    public bool IsValid(
        ShiftAssignment proposedAssignment,
        IEnumerable<ShiftAssignment> currentAssignments,
        ScheduleSettings settings,
        IEnumerable<TimeOff> timeOffs)
    {
        bool isOnLeave = timeOffs.Any(t =>
            proposedAssignment.Date >= t.StartDate &&
            proposedAssignment.Date <= t.EndDate);

        return !isOnLeave;
    }
}