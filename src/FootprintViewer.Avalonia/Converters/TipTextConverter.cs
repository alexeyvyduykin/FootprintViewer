using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Avalonia.Converters;

public class TipTextConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[1] is DrawingTip tip)
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

        if (values[1] == null)
        {
            return string.Empty;
        }

        return "error: text tip not find";

        // converter used for the wrong type
        //return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}

//public class TipTextConverter : IValueConverter
//{
//    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
//    {
//        if (parameter is DrawingTip tip)
//        {
//            var target = tip.Target;
//            var mode = tip.Mode;

//            if (mode is TipMode.Init)
//            {
//                if (target is TipTarget.Point)
//                {
//                    return Properties.Resources.TipInitPoint;
//                }
//                else if (target is TipTarget.Route)
//                {
//                    return Properties.Resources.TipInitRoute;
//                }
//                else if (target is TipTarget.Rectangle)
//                {
//                    return Properties.Resources.TipInitRectangle;
//                }
//                else if (target is TipTarget.Circle)
//                {
//                    return Properties.Resources.TipInitCircle;
//                }
//                else if (target is TipTarget.Polygon)
//                {
//                    return Properties.Resources.TipInitPolygon;
//                }
//            }
//            else if (mode is TipMode.BeginCreating)
//            {
//                if (target is TipTarget.Polygon)
//                {
//                    return Properties.Resources.TipBeginCreatingPolygon;
//                }
//            }
//            else if (mode is TipMode.HoverCreating)
//            {
//                if (target is TipTarget.Rectangle)
//                {
//                    return Properties.Resources.TipBeginCreatingRectangle;
//                }
//                else if (target is TipTarget.Circle)
//                {
//                    return Properties.Resources.TipBeginCreatingCircle;
//                }
//                else if (target is TipTarget.Route)
//                {
//                    return string.Empty;
//                }
//            }
//            else if (mode is TipMode.Creating)
//            {
//                if (target is TipTarget.Polygon)
//                {
//                    return Properties.Resources.TipCreatingPolygon;
//                }
//            }
//        }

//        if (parameter == null)
//        {
//            return string.Empty;
//        }

//        return "error: text tip not find";

//        // converter used for the wrong type
//        //return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
//    }

//    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
//    {
//        throw new NotSupportedException();
//    }
//}
