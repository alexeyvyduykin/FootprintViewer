using Avalonia.Data;
using Avalonia.Data.Converters;
using FootprintViewer.ViewModels;
using System;
using System.Globalization;

namespace FootprintViewer.Avalonia.Converters
{
    public class LanguageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is LanguageViewModel language)
            {
                return language.Code switch
                {
                    "en" => Properties.Resources.LanguageEnglish,
                    "ru" => Properties.Resources.LanguageRussian,
                    _ => throw new Exception(),
                };
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
