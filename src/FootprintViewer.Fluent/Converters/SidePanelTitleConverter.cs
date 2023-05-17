using Avalonia.Data;
using Avalonia.Data.Converters;
using System.Globalization;

namespace FootprintViewer.Fluent.Converters;

public class SidePanelTitleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string title)
        {
            switch (title)
            {
                case "Просмотр рабочей программы":
                    return "Footprint viewer";
                case "Поиск сцены":
                    return "Scene search";
                case "Просмотр наземных целей":
                    return "Ground target viewer";
                case "Просмотр наземных станций":
                    return "Ground station viewer";
                case "Просмотр спутников":
                    return "Satellite viewer";
                case "Пользовательские настройки":
                    return "User settings";
                case "Пользовательская геометрия":
                    return "User geometry";
                default:
                    break;
            }
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
