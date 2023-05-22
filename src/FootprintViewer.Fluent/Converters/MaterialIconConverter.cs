using Avalonia;
using Avalonia.Data.Converters;
using Material.Icons;
using System.Globalization;

namespace FootprintViewer.Fluent.Converters;

public class MaterialIconConverter : IValueConverter
{
    public static MaterialIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string key)
        {
            var iconKind = key switch
            {
                "AOI" => MaterialIconKind.VectorSquare,
                "Route" => MaterialIconKind.SetSquare,
                "AddRectangle" => MaterialIconKind.ShapeRectangleAdd,
                "AddPolygon" => MaterialIconKind.ShapePolygonAdd,
                "AddCircle" => MaterialIconKind.ShapeCircleAdd,
                "AddPoint" => MaterialIconKind.MapMarker,
                _ => default
            };

            return iconKind;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
