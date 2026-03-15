using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

public class CollapsiblePanel : ContentControl
{
    public static DependencyProperty IsCollapsedProperty =
        DependencyProperty.Register(
            nameof(IsCollapsed),
            typeof(bool),
            typeof(CollapsiblePanel),
            new PropertyMetadata(false));

    static CollapsiblePanel()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CollapsiblePanel),
            new FrameworkPropertyMetadata(typeof(CollapsiblePanel)));
    }

    public bool IsCollapsed
    {
        get => (bool)GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }
}