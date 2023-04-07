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
                    return Properties.Resources.ToolBarZoomIn;
                case "ZoomOut":
                    return Properties.Resources.ToolBarZoomOut;
                case "AddRectangle":
                    return Properties.Resources.ToolBarAddRectangle;
                case "AddPolygon":
                    return Properties.Resources.ToolBarAddPolygon;
                case "AddCircle":
                    return Properties.Resources.ToolBarAddCircle;
                case "Route":
                    return Properties.Resources.ToolBarRoute;
                case "MapBackgrounds":
                    return Properties.Resources.ToolBarMapBackgrounds;
                case "MapLayers":
                    return Properties.Resources.ToolBarMapLayers;
                case "Select":
                    return Properties.Resources.ToolBarSelect;
                case "Point":
                    return Properties.Resources.ToolBarPoint;
                case "Rectangle":
                    return Properties.Resources.ToolBarRectangle;
                case "Circle":
                    return Properties.Resources.ToolBarCircle;
                case "Polygon":
                    return Properties.Resources.ToolBarPolygon;
                case "Translate":
                    return Properties.Resources.ToolBarTranslate;
                case "Rotate":
                    return Properties.Resources.ToolBarRotate;
                case "Scale":
                    return Properties.Resources.ToolBarScale;
                case "Edit":
                    return Properties.Resources.ToolBarEdit;
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
