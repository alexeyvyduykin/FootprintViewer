using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace FootprintViewer.Fluent.Converters;

public class GroundStationModeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string item)
        {
            switch (item)
            {
                case "None":
                    return Properties.Resources.GroundStationModeNone;
                case "Equal":
                    return Properties.Resources.GroundStationModeEqual;
                case "Geometric":
                    return Properties.Resources.GroundStationModeGeometric;
                default:
                    break;
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string item)
        {
            var none = Properties.Resources.GroundStationModeNone;
            var equal = Properties.Resources.GroundStationModeEqual;
            var geometric = Properties.Resources.GroundStationModeGeometric;

            if (none?.Equals(item) == true)
            {
                return "None";
            }
            else if (equal?.Equals(item) == true)
            {
                return "Equal";
            }
            else if (geometric?.Equals(item) == true)
            {
                return "Geometric";
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}
