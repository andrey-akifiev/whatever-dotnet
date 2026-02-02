using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WhateverDotNet.TestApp.Components.Controls;

public class ExpanderToggleButton : ToggleButton
{
    public static DependencyProperty ColorMarkProperty =
        DependencyProperty.Register(
            nameof(ColorMark),
            typeof(Brush),
            typeof(ExpanderToggleButton),
            new PropertyMetadata(null));

    public static DependencyProperty ColorMarkSizeProperty =
        DependencyProperty.Register(
            nameof(ColorMarkSize),
            typeof(double),
            typeof(ExpanderToggleButton),
            new PropertyMetadata(3.0));

    static ExpanderToggleButton()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(ExpanderToggleButton),
                new FrameworkPropertyMetadata(typeof(ExpanderToggleButton)));
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