using System.Globalization;
using EasySchedule.Domain.Enums;

namespace EasySchedule.UI.Converters;

public class DraftToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ScheduleStatus status)
        {
            return status == ScheduleStatus.Draft;
        }
        return false;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}