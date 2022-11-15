using Avalonia.Data.Converters;
using FootprintViewer.ViewModels.Tips;
using System;
using System.Globalization;

namespace FootprintViewer.Avalonia.Converters;

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
                    return Properties.Resources.TipInitPoint;
                }
                else if (target is TipTarget.Route)
                {
                    return Properties.Resources.TipInitRoute;
                }
                else if (target is TipTarget.Rectangle)
                {
                    return Properties.Resources.TipInitRectangle;
                }
                else if (target is TipTarget.Circle)
                {
                    return Properties.Resources.TipInitCircle;
                }
                else if (target is TipTarget.Polygon)
                {
                    return Properties.Resources.TipInitPolygon;
                }
            }
            else if (mode is TipMode.BeginCreating)
            {
                if (target is TipTarget.Polygon)
                {
                    return Properties.Resources.TipBeginCreatingPolygon;
                }
            }
            else if (mode is TipMode.HoverCreating)
            {
                if (target is TipTarget.Rectangle)
                {
                    return Properties.Resources.TipBeginCreatingRectangle;
                }
                else if (target is TipTarget.Circle)
                {
                    return Properties.Resources.TipBeginCreatingCircle;
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
                    return Properties.Resources.TipCreatingPolygon;
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