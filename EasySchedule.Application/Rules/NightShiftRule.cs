using EasySchedule.Application.Interfaces.Rules;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;

namespace EasySchedule.Application.Rules;

public class NightShiftRule : IScheduleRule
{
    public string RuleName => "Zakaz pracy na nocki dla danego zawodu";
    public RuleSeverity Severity => RuleSeverity.Hard;

    public bool IsValid(
        ShiftAssignment proposedAssignment,
        IEnumerable<ShiftAssignment> currentAssignments,
        ScheduleSettings settings,
        IEnumerable<TimeOff> timeOffs)
    {
        var shift = proposedAssignment.ShiftType;
        var employee = proposedAssignment.Employee;

        if (shift == null || employee == null || employee.Profession == null)
            return true;

        if (shift.IsNightShift && !employee.Profession.CanWorkNightShifts)
        {
            return false;
        }

        return true;
    }
}