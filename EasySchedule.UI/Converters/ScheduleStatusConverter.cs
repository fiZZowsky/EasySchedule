using System.Globalization;
using EasySchedule.Domain.Enums;

namespace EasySchedule.UI.Converters;

public class ScheduleStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ScheduleStatus status)
        {
            return status switch
            {
                ScheduleStatus.Draft => "Szkic",
                ScheduleStatus.Published => "Opublikowany",
                ScheduleStatus.Archived => "Archiwalny",
                _ => status.ToString()
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}