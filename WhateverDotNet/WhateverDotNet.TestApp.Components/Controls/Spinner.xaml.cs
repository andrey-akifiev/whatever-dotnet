using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Interaction logic for Spinner.xaml
/// </summary>
public partial class Spinner : UserControl
{
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(
            nameof(Status),
            typeof(string),
            typeof(Spinner),
            new PropertyMetadata(null));

    public Spinner()
    {
        InitializeComponent();
    }

    public string? Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
}