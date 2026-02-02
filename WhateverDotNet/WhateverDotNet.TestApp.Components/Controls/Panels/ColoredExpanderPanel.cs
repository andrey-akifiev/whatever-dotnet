using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WhateverDotNet.TestApp.Components.Controls;

public class ColoredExpanderPanel : Expander
{
    public static DependencyProperty ColorMarkProperty =
        DependencyProperty.Register(
            nameof(ColorMark),
            typeof(Brush),
            typeof(ColoredExpanderPanel),
            new PropertyMetadata(null));

    public static DependencyProperty ColorMarkSizeProperty =
        DependencyProperty.Register(
            nameof(ColorMarkSize),
            typeof(double),
            typeof(ColoredExpanderPanel),
            new PropertyMetadata(3.0));

    static ColoredExpanderPanel()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(ColoredExpanderPanel),
                new FrameworkPropertyMetadata(typeof(ColoredExpanderPanel)));
    }

    public Brush? ColorMark
    {
        get => (Brush?)GetValue(ColorMarkProperty);
        set => SetValue(ColorMarkProperty, value);
    }

    public double ColorMarkSize
    {
        get => (double)GetValue(ColorMarkSizeProperty);
        set => SetValue(ColorMarkSizeProperty, value);
    }
}