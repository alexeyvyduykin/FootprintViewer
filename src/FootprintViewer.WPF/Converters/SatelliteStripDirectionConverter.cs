using FootprintViewer.Data;
using System;
using System.Windows.Data;

namespace FootprintViewer.WPF.Converters
{
    [ValueConversion(typeof(SatelliteStripDirection), typeof(string))]
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
