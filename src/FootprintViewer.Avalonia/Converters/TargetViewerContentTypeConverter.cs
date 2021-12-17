using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using FootprintViewer.ViewModels;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace FootprintViewer.Avalonia.Converters
{
    public class TargetViewerContentTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((TargetViewerContentType)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Enum.Parse(typeof(TargetViewerContentType), (string)value);
        }
    }
}
