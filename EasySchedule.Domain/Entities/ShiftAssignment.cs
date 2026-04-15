namespace EasySchedule.Domain.Entities;

public class ShiftAssignment
{
    public int Id { get; private set; }

    public int ScheduleId { get; private set; }
    public Schedule? Schedule { get; private set; }

    public int EmployeeId { get; private set; }
    public Employee? Employee { get; private set; }

    public int ShiftTypeId { get; private set; }
    public ShiftType? ShiftType { get; private set; }

    public DateOnly Date { get; private set; }

    private ShiftAssignment() { }

    public ShiftAssignment(int scheduleId, int employeeId, int shiftTypeId, DateOnly date)
    {
        ScheduleId = scheduleId;
        EmployeeId = employeeId;
        ShiftTypeId = shiftTypeId;
        Date = date;
    }

    public void UpdateAssignment(int shiftTypeId, DateOnly date)
    {
        ShiftTypeId = shiftTypeId;
        Date = date;
    }
}