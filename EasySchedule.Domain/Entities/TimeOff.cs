using EasySchedule.Domain.Enums;

namespace EasySchedule.Domain.Entities;

public class TimeOff
{
    public int Id { get; private set; }
    public int EmployeeId { get; private set; }
    public Employee? Employee { get; private set; }

    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public TimeOffType Type { get; private set; }
    public string? Comments { get; private set; }

    private TimeOff() { }

    public TimeOff(int employeeId, DateOnly startDate, DateOnly endDate, TimeOffType type, string? comments = null)
    {
        EmployeeId = employeeId;
        StartDate = startDate;
        EndDate = endDate;
        Type = type;
        Comments = comments;
    }

    public void UpdateDetails(DateOnly startDate, DateOnly endDate, TimeOffType type, string? comments)
    {
        StartDate = startDate;
        EndDate = endDate;
        Type = type;
        Comments = comments;
    }
}