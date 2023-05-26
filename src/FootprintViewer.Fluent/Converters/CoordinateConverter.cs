using Avalonia.Data;
using Avalonia.Data.Converters;
using NetTopologySuite.Geometries;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class CoordinateConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Coordinate coordinate)
        {
            return $"{coordinate.X:0.00}° {coordinate.Y:0.00}°";
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
