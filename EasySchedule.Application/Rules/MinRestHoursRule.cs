using EasySchedule.Application.Interfaces.Rules;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;

namespace EasySchedule.Application.Rules;

public class MinRestHoursRule : IScheduleRule
{
    public string RuleName => "Minimalny czas odpoczynku między zmianami";
    public RuleSeverity Severity => RuleSeverity.High;

    public bool IsValid(
        ShiftAssignment proposedAssignment,
        IEnumerable<ShiftAssignment> currentAssignments,
        ScheduleSettings settings,
        IEnumerable<TimeOff> timeOffs)
    {
        if (proposedAssignment.ShiftType == null) return false;

        var proposedStart = proposedAssignment.Date.ToDateTime(proposedAssignment.ShiftType.StartTime);

        foreach (var existingShift in currentAssignments)
        {
            if (existingShift.ShiftType == null) continue;

            var existingStart = existingShift.Date.ToDateTime(existingShift.ShiftType.StartTime);
            var existingEnd = existingStart.Add(existingShift.ShiftType.Duration);

            var restBefore = (proposedStart - existingEnd).TotalHours;

            var proposedEnd = proposedStart.Add(proposedAssignment.ShiftType.Duration);
            var restAfter = (existingStart - proposedEnd).TotalHours;

            if ((restBefore >= 0 && restBefore < settings.MinRestHoursBetweenShifts) ||
                (restAfter >= 0 && restAfter < settings.MinRestHoursBetweenShifts))
            {
                return false;
            }
        }

        return true;
    }
}