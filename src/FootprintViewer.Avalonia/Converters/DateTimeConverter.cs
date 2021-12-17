using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace FootprintViewer.Avalonia.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dateTime = (DateTime)value;

            return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateTimePickerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var dateTime = (DateTime)value;

            return new DateTimeOffset(dateTime);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTimeOffset = (DateTimeOffset)value;

            return dateTimeOffset.DateTime;            
        }
    }
}
