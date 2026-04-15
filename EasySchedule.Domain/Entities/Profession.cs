namespace EasySchedule.Domain.Entities;

public class Profession
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();

    private Profession() { }

    public Profession(string name, string description = "")
    {
        Name = name;
        Description = description;
    }

    public void UpdateDetails(string name, string description)
    {
        Name = name;
        Description = description;
    }
}