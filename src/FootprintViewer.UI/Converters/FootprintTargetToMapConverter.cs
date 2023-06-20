using Avalonia;
using Avalonia.Data.Converters;
using FootprintViewer.UI.ViewModels.SidePanel.Tabs;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class FootprintTargetToMapConverter : IValueConverter
{
    public static FootprintTargetToMapConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FootprintTabViewModel tab)
        {
            return tab.TargetToMap;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
