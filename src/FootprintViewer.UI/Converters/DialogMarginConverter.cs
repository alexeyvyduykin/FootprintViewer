using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class DialogMarginConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double size)
        {
            var margin = Double.TryParse(parameter?.ToString(), out var d) ? d : 0.0;

            return size - margin - margin;
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
