using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Interaction logic for RegularExpanderPanel.xaml
/// </summary>
public partial class RegularExpanderPanel : UserControl
{
    public static readonly DependencyProperty CollapsibleContentProperty =
        DependencyProperty.Register(
            nameof(CollapsibleContent),
            typeof(object),
            typeof(RegularExpanderPanel),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool),
            typeof(RegularExpanderPanel),
            new PropertyMetadata(false));

    public static readonly DependencyProperty TitleContentProperty =
        DependencyProperty.Register(
            nameof(TitleContent),
            typeof(object),
            typeof(RegularExpanderPanel),
            new PropertyMetadata(null));

    public RegularExpanderPanel()
    {
        InitializeComponent();
    }

    public object CollapsibleContent
    {
        get => GetValue(CollapsibleContentProperty);
        set => SetValue(CollapsibleContentProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public object TitleContent
    {
        get => GetValue(TitleContentProperty);
        set => SetValue(TitleContentProperty, value);
    }
}