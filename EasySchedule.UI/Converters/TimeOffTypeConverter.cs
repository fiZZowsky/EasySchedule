using System.Globalization;
using EasySchedule.Domain.Enums;

namespace EasySchedule.UI.Converters;

public class TimeOffTypeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeOffType type)
        {
            return type switch
            {
                TimeOffType.Vacation => "Urlop wypoczynkowy",
                TimeOffType.SickLeave => "Zwolnienie lekarskie (L4)",
                TimeOffType.OnDemand => "Urlop na żądanie",
                TimeOffType.Unpaid => "Urlop bezpłatny",
                TimeOffType.Other => "Inne",
                _ => type.ToString()
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}