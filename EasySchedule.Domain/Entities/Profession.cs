namespace EasySchedule.Domain.Entities;

public class Profession
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    public bool CanWorkNightShifts { get; private set; } = true;

    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private Profession() { }

    public Profession(string name, string description = "", bool canWorkNightShifts = true)
    {
        Name = name;
        Description = description;
        CanWorkNightShifts = canWorkNightShifts;
    }

    public void UpdateDetails(string name, string description, bool canWorkNightShifts)
    {
        Name = name;
        Description = description;
        CanWorkNightShifts = canWorkNightShifts;
    }
}