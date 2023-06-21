using Avalonia;
using Avalonia.Data.Converters;
using FootprintViewer.UI.ViewModels.Navigation;
using System.Globalization;

namespace FootprintViewer.UI.Converters;

public class DialogCurrentPageConverter : IValueConverter
{
    public static DialogCurrentPageConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is RoutableViewModel vm)
        {
            if (parameter is string param)
            {
                if (param == "IsActive")
                {
                    return vm.IsActive;
                }
                else if (param == "IsBusy")
                {
                    return vm.IsBusy;
                }
                else if (param == "EnableBack")
                {
                    return vm.EnableBack;
                }
                else if (param == "EnableCancel")
                {
                    return vm.EnableCancel;
                }
                else if (param == "EnableCancelOnPressed")
                {
                    return vm.EnableCancelOnPressed;
                }
                else if (param == "EnableCancelOnEscape")
                {
                    return vm.EnableCancelOnEscape;
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

public class Converter2 : IValueConverter
{
    public static Converter2 Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {



        return value;

        return AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
