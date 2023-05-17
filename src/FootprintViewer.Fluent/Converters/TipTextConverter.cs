using Avalonia.Data.Converters;
using FootprintViewer.Fluent.ViewModels.Tips;
using System.Globalization;

namespace FootprintViewer.Fluent.Converters;

public class TipTextConverter1 : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CustomTipViewModel tip)
        {
            var target = tip.Target;
            var mode = tip.Mode;

            if (mode is TipMode.Init)
            {
                if (target is TipTarget.Point)
                {
                    return "Click to draw a point";
                }
                else if (target is TipTarget.Route)
                {
                    return "Click to start measurement";
                }
                else if (target is TipTarget.Rectangle)
                {
                    return "Click and drag to draw a rectangle";
                }
                else if (target is TipTarget.Circle)
                {
                    return "Click and drag to draw a circle";
                }
                else if (target is TipTarget.Polygon)
                {
                    return "Click and drag to draw a polygon";
                }
            }
            else if (mode is TipMode.BeginCreating)
            {
                if (target is TipTarget.Polygon)
                {
                    return "Click to continue drawing the shape";
                }
            }
            else if (mode is TipMode.HoverCreating)
            {
                if (target is TipTarget.Rectangle)
                {
                    return "Click to finish drawing";
                }
                else if (target is TipTarget.Circle)
                {
                    return "Click to finish drawing";
                }
                else if (target is TipTarget.Route)
                {
                    return string.Empty;
                }
            }
            else if (mode is TipMode.Creating)
            {
                if (target is TipTarget.Polygon)
                {
                    return "Click on the first point to close this shape";
                }
            }
        }

        return "error: text tip not find";

        // converter used for the wrong type
        //return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}