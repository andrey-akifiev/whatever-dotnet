using System.Windows;

namespace WhateverDotNet.TestApp.Components.Helpers;

public static class ValidationHelper
{
    public static readonly DependencyProperty HasWarningProperty =
        DependencyProperty.RegisterAttached(
            "HasWarning",
            typeof(bool),
            typeof(ValidationHelper),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

    public static readonly DependencyProperty WarningContentProperty =
        DependencyProperty.RegisterAttached(
            "WarningContent",
            typeof(string),
            typeof(ValidationHelper),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    public static bool GetHasWarning(DependencyObject obj)
    {
        return (bool)obj.GetValue(HasWarningProperty);
    }

    public static string GetWarningContent(DependencyObject obj)
    {
        return (string)obj.GetValue(WarningContentProperty);
    }

    public static void SetHasWarning(DependencyObject obj, bool value)
    {
        obj.SetValue(HasWarningProperty, value);
    }

    public static void SetWarningContent(DependencyObject obj, string value)
    {
        obj.SetValue(WarningContentProperty, value);
    }
}