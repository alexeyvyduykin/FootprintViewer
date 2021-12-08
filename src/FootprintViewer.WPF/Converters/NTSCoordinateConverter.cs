using NetTopologySuite.Geometries;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FootprintViewer.WPF.Converters
{
    [ValueConversion(typeof(Coordinate), typeof(string))]
    public class NTSCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var coord = (Coordinate)value;

            return $"{coord.X:0.00}° {coord.Y:0.00}°";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
