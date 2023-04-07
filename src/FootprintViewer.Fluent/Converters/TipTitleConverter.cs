using Avalonia.Data.Converters;
using FootprintViewer.Fluent.ViewModels.Tips;
using System.Globalization;

namespace FootprintViewer.Fluent.Converters;

public class TipTitleConverter1 : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CustomTipViewModel tip)
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

        return "error: title tip not find";

        // converter used for the wrong type
        //return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

