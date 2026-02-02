using CommunityToolkit.Mvvm.Input;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Interaction logic for CollapsiblePanel.xaml
/// </summary>
public partial class CollapsiblePanel : UserControl
{
    public static readonly DependencyProperty CollapsibleContentProperty =
        DependencyProperty.Register(
            nameof(CollapsibleContent),
            typeof(object),
            typeof(CollapsiblePanel),
            new PropertyMetadata(null));

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(
            nameof(Header),
            typeof(string),
            typeof(CollapsiblePanel));
    
    public static readonly DependencyProperty IsExpandedProperty =
        DependencyProperty.Register(
            nameof(IsExpanded),
            typeof(bool),
            typeof(CollapsiblePanel),
            new PropertyMetadata(false));

    public CollapsiblePanel()
    {
        InitializeComponent();
        ToggleCommand = new RelayCommand(() => IsExpanded = !IsExpanded);
    }

    public object CollapsibleContent
    {
        get => GetValue(CollapsibleContentProperty);
        set => SetValue(CollapsibleContentProperty, value);
    }

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public ICommand ToggleCommand { get; }
}