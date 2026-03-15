using System.Globalization;
using System.Windows.Data;

namespace WhateverDotNet.TestApp.Components.Converters;

[ValueConversion(typeof(bool), typeof(bool))]
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType == typeof(bool))
        {
            return !(bool)value;
        }

        if (targetType == typeof(Nullable<bool>))
        {
            return !((bool?)value ?? false);
        }

        throw new InvalidOperationException($"The target '{targetType}' must be a boolean");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(bool))
        {
            throw new InvalidOperationException($"The target '{targetType}' must be a boolean");
        }

        return !(bool)value;
    }
}