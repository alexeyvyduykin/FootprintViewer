using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace FootprintViewer.WPF.Converters
{
    [ValueConversion(typeof(FootprintViewerContentType), typeof(string))]
    public class FootprintViewerContentTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((FootprintViewerContentType)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.Parse(typeof(FootprintViewerContentType), (string)value);
        }
    }
}
