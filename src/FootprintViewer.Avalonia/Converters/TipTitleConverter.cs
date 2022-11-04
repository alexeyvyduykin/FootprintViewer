using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using FootprintViewer.ViewModels;

namespace FootprintViewer.Avalonia.Converters;

public class TipTitleConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values[1] is DrawingTip tip)
        {
            var target = tip.Target;
            var mode = tip.Mode;
            var value2 = tip.Value;

            if (mode is TipMode.Init)
            {
                return string.Empty;
            }
            else if (mode is TipMode.BeginCreating)
            {
                return string.Empty;
            }
            else if (mode is TipMode.HoverCreating)
            {
                if (target is TipTarget.Rectangle || target is TipTarget.Circle)
                {
                    return $"{Properties.Resources.Area}: {value2}";
                }
                else if (target is TipTarget.Route)
                {
                    return $"{Properties.Resources.Distance}: {value2}";
                }
            }
            else if (mode is TipMode.Creating)
            {
                if (target is TipTarget.Polygon)
                {
                    return $"{Properties.Resources.Area}: {value2}";
                }
            }
        }

        if (values[1] == null)
        {
            return string.Empty;
        }

        return "error: title tip not find";

        // converter used for the wrong type
        //return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}

//public class TipTitleConverter : IValueConverter
//{
//    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
//    {
//        if (parameter is DrawingTip tip)
//        {
//            var target = tip.Target;
//            var mode = tip.Mode;
//            var value2 = tip.Value;

//            if (mode is TipMode.Init)
//            {
//                return string.Empty;
//            }
//            else if (mode is TipMode.BeginCreating)
//            {
//                return string.Empty;
//            }
//            else if (mode is TipMode.HoverCreating)
//            {
//                if (target is TipTarget.Rectangle || target is TipTarget.Circle)
//                {
//                    return $"{Properties.Resources.Area}: {value2}";
//                }
//                else if (target is TipTarget.Route)
//                {
//                    return $"{Properties.Resources.Distance}: {value2}";
//                }
//            }
//            else if (mode is TipMode.Creating)
//            {
//                if (target is TipTarget.Polygon)
//                {
//                    return $"{Properties.Resources.Area}: {value2}";
//                }
//            }
//        }

//        if (parameter == null)
//        {
//            return string.Empty;
//        }

//        return "error: title tip not find";

//        // converter used for the wrong type
//        //return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
//    }

//    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
//    {
//        throw new NotSupportedException();
//    }
//}

