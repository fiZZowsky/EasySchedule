namespace EasySchedule.Domain.Entities;

public class ShiftRequirement
{
    public int Id { get; private set; }

    public int ScheduleId { get; private set; }
    public Schedule? Schedule { get; private set; }

    public int ShiftTypeId { get; private set; }
    public ShiftType? ShiftType { get; private set; }

    public DateOnly? SpecificDate { get; private set; }

    public int RequiredEmployeeCount { get; private set; }

    private ShiftRequirement() { }

    public ShiftRequirement(int scheduleId, int shiftTypeId, int requiredEmployeeCount, DateOnly? specificDate = null)
    {
        ScheduleId = scheduleId;
        ShiftTypeId = shiftTypeId;
        RequiredEmployeeCount = requiredEmployeeCount;
        SpecificDate = specificDate;
    }

    public void UpdateCount(int requiredCount)
    {
        RequiredEmployeeCount = requiredCount;
    }
}