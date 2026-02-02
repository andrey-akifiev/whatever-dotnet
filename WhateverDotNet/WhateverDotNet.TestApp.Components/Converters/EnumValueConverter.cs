using System.Globalization;
using System.Windows.Data;

namespace WhateverDotNet.TestApp.Components.Converters;

[ValueConversion(typeof(Enum), typeof(string))]
public class EnumValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var enumValue = value as Enum;
        return enumValue?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}