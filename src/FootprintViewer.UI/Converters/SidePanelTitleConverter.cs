using Avalonia;
using Avalonia.Data.Converters;
using FootprintViewer.UI.Controls;
using FootprintViewer.UI.ViewModels.SidePanel;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class SidePanelTitleConverter : IValueConverter
{
    public static SidePanelTitleConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SidePanelTabViewModel item)
        {
            return item.Title;
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
