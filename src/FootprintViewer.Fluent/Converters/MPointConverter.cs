using Avalonia.Data;
using Avalonia.Data.Converters;
using Mapsui;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class MPointConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MPoint point)
        {
            var (lon, lat) = (point.X, point.Y);

            var lonStr = (lon >= 0.0) ? $"{lon:F5}°E" : $"{Math.Abs(lon):F5}°W";

            var latStr = (lat >= 0.0) ? $"{lat:F5}°N" : $"{Math.Abs(lat):F5}°S";

            return $"{lonStr} {latStr}";
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

