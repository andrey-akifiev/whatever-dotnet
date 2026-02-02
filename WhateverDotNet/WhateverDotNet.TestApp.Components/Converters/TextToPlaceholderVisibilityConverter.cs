using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WhateverDotNet.TestApp.Components.Converters;

public class TextToPlaceholderVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.IsNullOrEmpty(value as string)
            ? Visibility.Visible
            : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}