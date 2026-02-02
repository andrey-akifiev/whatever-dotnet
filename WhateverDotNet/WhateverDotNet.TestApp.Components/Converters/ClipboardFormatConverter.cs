using System.Globalization;
using System.Windows.Data;

using WhateverDotNet.TestApp.Components.Commands;

namespace WhateverDotNet.TestApp.Components.Converters;

public class ClipboardFormatConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var value = values[0]?.ToString();
        var template = values[1]?.ToString();

        return new CopyToCliboardCommandParameters
        {
            Template = template,
            Value = value,
        };
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}