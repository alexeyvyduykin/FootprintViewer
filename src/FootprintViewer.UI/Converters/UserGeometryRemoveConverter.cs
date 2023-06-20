using Avalonia;
using Avalonia.Data.Converters;
using FootprintViewer.UI.ViewModels.SidePanel.Tabs;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class UserGeometryRemoveConverter : IValueConverter
{
    public static UserGeometryRemoveConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserGeometryTabViewModel tab)
        {
            return tab.Remove;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
