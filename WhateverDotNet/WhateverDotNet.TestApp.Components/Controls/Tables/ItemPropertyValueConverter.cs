using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace WhateverDotNet.TestApp.Components.Controls;

public sealed class ItemPropertyValueConverter : IMultiValueConverter
{
    private static readonly ConcurrentDictionary<(Type type, string propertyName), PropertyInfo?> PropertyCache = new();

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2)
        {
            return Binding.DoNothing;
        }

        object? item = values[0];
        string? propertyName = values[1] as string;

        if (item == null || string.IsNullOrWhiteSpace(propertyName))
        {
            return string.Empty;
        }

        var key = (item.GetType(), propertyName);
        PropertyInfo? property = PropertyCache.GetOrAdd(
            key,
            static k => k.type.GetProperty(k.propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));

        if (property == null)
        {
            return string.Empty;
        }

        try
        {
            object? value = property.GetValue(item);
            return value?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}