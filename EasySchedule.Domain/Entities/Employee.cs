namespace EasySchedule.Domain.Entities;

public class Employee
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Surname { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public int ProfessionId { get; private set; }
    public Profession? Profession { get; private set; }

    private readonly List<TimeOff> _timeOffs = new();
    public IReadOnlyCollection<TimeOff> TimeOffs => _timeOffs.AsReadOnly();

    private Employee() { }

    public Employee(string name, string surname, string phoneNumber, int professionId)
    {
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;
        ProfessionId = professionId;
    }

    public void UpdateDetails(string name, string surname, string phoneNumber)
    {
        Name = name;
        Surname = surname;
        PhoneNumber = phoneNumber;
    }

    public void ChangeProfession(int newProfessionId)
    {
        ProfessionId = newProfessionId;
    }
}