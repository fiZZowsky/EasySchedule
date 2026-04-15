namespace EasySchedule.Domain.Entities;

public class ScheduleSettings
{
    public int Id { get; private set; }

    public int MaxConsecutiveWorkingDays { get; private set; } = 5;
    public int MinRestHoursBetweenShifts { get; private set; } = 11;
    public int MaxShiftsPerWeek { get; private set; } = 5;

    private ScheduleSettings() { }

    public ScheduleSettings(int maxConsecutiveWorkingDays, int minRestHoursBetweenShifts, int maxShiftsPerWeek)
    {
        MaxConsecutiveWorkingDays = maxConsecutiveWorkingDays;
        MinRestHoursBetweenShifts = minRestHoursBetweenShifts;
        MaxShiftsPerWeek = maxShiftsPerWeek;
    }

    public void UpdateSettings(int maxConsecutiveDays, int minRestHours, int maxShifts)
    {
        MaxConsecutiveWorkingDays = maxConsecutiveDays;
        MinRestHoursBetweenShifts = minRestHours;
        MaxShiftsPerWeek = maxShifts;
    }
}