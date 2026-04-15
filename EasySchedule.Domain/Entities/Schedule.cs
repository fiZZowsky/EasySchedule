using EasySchedule.Domain.Enums;

namespace EasySchedule.Domain.Entities;

public class Schedule
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public ScheduleStatus Status { get; private set; }

    private readonly List<ShiftAssignment> _shiftAssignments = new();
    public IReadOnlyCollection<ShiftAssignment> ShiftAssignments => _shiftAssignments.AsReadOnly();
    public int ProfessionId { get; private set; }
    public Profession? Profession { get; private set; }
    private readonly List<ShiftRequirement> _shiftRequirements = new();
    public IReadOnlyCollection<ShiftRequirement> ShiftRequirements => _shiftRequirements.AsReadOnly();

    private Schedule() { }

    public Schedule(string name, DateOnly startDate, DateOnly endDate, int professionId)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        ProfessionId = professionId;
        Status = ScheduleStatus.Draft;
    }

    public void UpdateDetails(string name, DateOnly startDate, DateOnly endDate, int professionId)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        ProfessionId = professionId;
    }

    public void ChangeStatus(ScheduleStatus newStatus)
    {
        Status = newStatus;
    }
}