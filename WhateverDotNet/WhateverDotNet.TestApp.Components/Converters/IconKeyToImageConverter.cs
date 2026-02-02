using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WhateverDotNet.TestApp.Components.Converters;

public class IconKeyToImageConverter : IMultiValueConverter
{
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2)
        {
            return null;
        }

        if (values[0] is not string key || string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        // TODO: Remove this branch when all usages are updated
        if (values[1] is bool b)
        {
            bool isEnabled = b;

            string suffix = isEnabled ? "colored" : "grayed";
            string uri = $"pack://application:,,,/WhateverDotNet.TestApp.Components;component/Assets/Icons/{key}_{suffix}.png";

            return new BitmapImage(new Uri(uri, UriKind.Absolute));
        }
        
        if (values[1] is string state)
        {
            string uri = $"pack://application:,,,/WhateverDotNet.TestApp.Components;component/Assets/Icons/{key}_{state}.png";

            return new BitmapImage(new Uri(uri, UriKind.Absolute));
        }

        return null;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}