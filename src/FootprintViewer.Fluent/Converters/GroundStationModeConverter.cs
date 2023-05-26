using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class GroundStationModeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string item)
        {
            switch (item)
            {
                case "None":
                    return "None";
                case "Equal":
                    return "Equal";
                case "Geometric":
                    return "Geometric";
                default:
                    break;
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
