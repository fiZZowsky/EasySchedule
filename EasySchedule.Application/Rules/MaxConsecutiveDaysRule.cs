using EasySchedule.Application.Interfaces.Rules;
using EasySchedule.Domain.Entities;
using EasySchedule.Domain.Enums;

namespace EasySchedule.Application.Rules;

public class MaxConsecutiveDaysRule : IScheduleRule
{
    public string RuleName => "Maksymalna liczba pracujących dni z rzędu";
    public RuleSeverity Severity => RuleSeverity.Medium;

    public bool IsValid(
        ShiftAssignment proposedAssignment,
        IEnumerable<ShiftAssignment> currentAssignments,
        ScheduleSettings settings,
        IEnumerable<TimeOff> timeOffs)
    {
        var allWorkingDates = currentAssignments.Select(a => a.Date).ToList();
        allWorkingDates.Add(proposedAssignment.Date);

        allWorkingDates = allWorkingDates.Distinct().OrderBy(d => d).ToList();

        int consecutiveDays = 1;
        int maxConsecutiveFound = 1;

        for (int i = 1; i < allWorkingDates.Count; i++)
        {
            if (allWorkingDates[i].DayNumber - allWorkingDates[i - 1].DayNumber == 1)
            {
                consecutiveDays++;
                if (consecutiveDays > maxConsecutiveFound)
                    maxConsecutiveFound = consecutiveDays;
            }
            else
            {
                consecutiveDays = 1;
            }

            if (maxConsecutiveFound > settings.MaxConsecutiveWorkingDays)
            {
                return false;
            }
        }

        return true;
    }
}