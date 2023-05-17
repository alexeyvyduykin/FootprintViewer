using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace FootprintViewer.Fluent.Converters;

public class ToolBarToolTipConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string tag)
        {
            switch (tag)
            {
                case "ZoomIn":
                    return "Zoom in";
                case "ZoomOut":
                    return "Zoom out";
                case "AddRectangle":
                    return "Draw rectangle AOI";
                case "AddPolygon":
                    return "Draw polygon AOI";
                case "AddCircle":
                    return "Draw circle AOI";
                case "Route":
                    return "Measure route";
                case "MapBackgrounds":
                    return "List map backgrounds";
                case "MapLayers":
                    return "List map layers";
                case "Select":
                    return "Select geometry";
                case "Point":
                    return "Draw point";
                case "Rectangle":
                    return "Draw rectangle";
                case "Circle":
                    return "Draw circle";
                case "Polygon":
                    return "Draw polygon";
                case "Translate":
                    return "Translate geometry";
                case "Rotate":
                    return "Rotate geometry";
                case "Scale":
                    return "Scale geometry";
                case "Edit":
                    return "Edit geometry";
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
