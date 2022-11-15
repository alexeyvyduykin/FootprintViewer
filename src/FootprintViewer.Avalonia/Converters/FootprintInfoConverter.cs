﻿using Avalonia.Data;
using Avalonia.Data.Converters;
using FootprintViewer.ViewModels.SidePanel.Items;
using System;
using System.Globalization;

namespace FootprintViewer.Avalonia.Converters;

public class FootprintInfoNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FootprintViewModel footprint)
        {
            return $"{footprint.SatelliteName} ({footprint.Node} - {footprint.Direction})";
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public class FootprintInfoTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FootprintViewModel footprint)
        {
            return $"{footprint.Begin: dd.MM.yyyy HH:mm:ss} ({footprint.Duration} sec)";
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}