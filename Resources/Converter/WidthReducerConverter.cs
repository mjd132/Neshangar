using System.Globalization;
using System.Windows.Data;

namespace Neshangar.Resources.Converter;

public class WidthReducerConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double listBoxWidth)
        {
            return listBoxWidth - 20;
        }
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}