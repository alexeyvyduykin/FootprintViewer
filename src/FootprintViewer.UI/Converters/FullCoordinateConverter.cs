using Avalonia.Data;
using Avalonia.Data.Converters;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class FullCoordinateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Coordinate coordinate)
        {
            return $"Lon:{coordinate.X,7: 0.00;-0.00; 0.00}° Lat:{coordinate.Y,6: 0.00;-0.00; 0.00}°";
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
