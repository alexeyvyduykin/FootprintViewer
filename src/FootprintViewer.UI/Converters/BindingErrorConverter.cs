using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using FootprintViewer.UI.ViewModels.SidePanel.Tabs;
using System.Globalization;
using System.Windows.Input;

namespace FootprintViewer.UI.Converters;

public class GroundTargetTabConverter : IValueConverter
{
    public static GroundTargetTabConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UserControl userControl)
        {
            if (userControl.DataContext is GroundTargetTabViewModel vm)
            {
                if (parameter is string param)
                {
                    if (param == "Enter")
                    {
                        return vm.Enter;
                    }
                    if (param == "Leave")
                    {
                        return vm.Leave;
                    }
                }
            }
        }

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}