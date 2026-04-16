using EasySchedule.Domain.Enums;

namespace EasySchedule.Domain.Entities;

public class Schedule
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ScheduleStatus Status { get; set; }

    private readonly List<ShiftAssignment> _shiftAssignments = new();
    public IReadOnlyCollection<ShiftAssignment> ShiftAssignments => _shiftAssignments.AsReadOnly();
    public int ProfessionId { get; set; }
    public Profession? Profession { get; set; }
    private readonly List<ShiftRequirement> _shiftRequirements = new();
    public IReadOnlyCollection<ShiftRequirement> ShiftRequirements => _shiftRequirements.AsReadOnly();

    private Schedule() { }

    public Schedule(string name, DateOnly startDate, DateOnly endDate, int professionId, ScheduleStatus status = ScheduleStatus.Draft)
    {
        Name = name;
        StartDate = startDate;
        EndDate = endDate;
        ProfessionId = professionId;
        Status = status;
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