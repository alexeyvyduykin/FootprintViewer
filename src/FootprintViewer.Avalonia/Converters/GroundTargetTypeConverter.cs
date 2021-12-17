using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace FootprintViewer.Avalonia.Converters
{
    public class GroundTargetTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((GroundTargetType)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.Parse(typeof(GroundTargetType), (string)value);
        }
    }
}
