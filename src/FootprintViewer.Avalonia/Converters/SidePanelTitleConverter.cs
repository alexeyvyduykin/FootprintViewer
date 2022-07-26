using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FootprintViewer.Avalonia.Converters
{
    public class SidePanelTitleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string title)
            {
                switch (title)
                {
                    case "Просмотр рабочей программы":
                        return Properties.Resources.FootprintTabTitle;
                    case "Поиск сцены":
                        return Properties.Resources.FootprintPreviewTabTitle;
                    case "Просмотр наземных целей":
                        return Properties.Resources.GroundTargetTabTitle;
                    case "Просмотр наземных станций":
                        return Properties.Resources.GroundStationTabTtile;
                    case "Просмотр спутников":
                        return Properties.Resources.SatelliteTabTitle;
                    case "Пользовательские настройки":
                        return Properties.Resources.UserSettingsTabTitle;
                    case "Пользовательская геометрия":
                        return Properties.Resources.UserGeometryTabTitle;
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
}
