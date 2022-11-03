using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using FootprintViewer.Styles;
using System;
using System.Globalization;

namespace FootprintViewer.Avalonia.Converters;

public class SolidColorBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Mapsui.Styles.Color color)
        {
            return new SolidColorBrush()
            {
                Color = Color.FromRgb((byte)color.R, (byte)color.G, (byte)color.B)
            };
        }

        if (value is string name)
        {
            if (string.IsNullOrWhiteSpace(name) == true)
            {
                return new SolidColorBrush() { Color = Colors.Black };
            }

            var clr = LayerStyleManager.SatellitePalette.PickColor(name);

            return new SolidColorBrush()
            {
                Color = Color.FromRgb(clr.R, clr.G, clr.B)
            };
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
