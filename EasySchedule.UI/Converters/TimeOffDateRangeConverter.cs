using System.Globalization;

namespace EasySchedule.UI.Converters;

public class TimeOffDateRangeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 3) return string.Empty;
        if (values[0] is not DateOnly start || values[1] is not DateOnly end)
            return string.Empty;

        string typeInfo = values[2]?.ToString() ?? string.Empty;

        if (start == end)
        {
            return $"{start:dd.MM.yyyy} ({typeInfo})";
        }

        return $"{start:dd.MM.yyyy} - {end:dd.MM.yyyy} ({typeInfo})";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}