using Avalonia;
using Avalonia.Data.Converters;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class PathIconConverter : IValueConverter
{
    public static PathIconConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string key)
        {
            var icon = key switch
            {
                "Point" => "fg-poi",
                "Route" => "fg-route",
                "Area" => "fg-regular-shape-o",
                _ => default
            };

            if (icon is null)
            {
                return AvaloniaProperty.UnsetValue;
            }

            if (Application.Current?.Resources.TryGetResource(icon, null, out var resource) == true)
            {
                return resource;
            }
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
