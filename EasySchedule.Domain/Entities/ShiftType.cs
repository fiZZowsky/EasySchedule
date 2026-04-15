namespace EasySchedule.Domain.Entities;

public class ShiftType
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }

    public string ShortName { get; private set; } = string.Empty;

    public TimeSpan Duration
    {
        get
        {
            if (EndTime > StartTime)
            {
                return EndTime - StartTime;
            }
            else
            {
                var hoursBeforeMidnight = new TimeOnly(23, 59, 59, 999) - StartTime + TimeSpan.FromMilliseconds(1);
                var hoursAfterMidnight = EndTime - TimeOnly.MinValue;
                return hoursBeforeMidnight + hoursAfterMidnight;
            }
        }
    }

    private ShiftType() { }

    public ShiftType(string name, string shortName, TimeOnly startTime, TimeOnly endTime)
    {
        Name = name;
        ShortName = shortName;
        StartTime = startTime;
        EndTime = endTime;
    }

    public void UpdateDetails(string name, string shortName, TimeOnly startTime, TimeOnly endTime)
    {
        Name = name;
        ShortName = shortName;
        StartTime = startTime;
        EndTime = endTime;
    }
}