using FootprintViewer.Data;
using System;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace FootprintViewer.Avalonia.Converters
{
    public class SatelliteStripDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((SatelliteStripDirection)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.Parse(typeof(SatelliteStripDirection), (string)value);
        }
    }
}
