using System.Globalization;
using System.Windows.Data;

namespace WhateverDotNet.TestApp.Components.Converters;

public class NegateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is double d ? -d : 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}