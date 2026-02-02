using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Interaction logic for IconButton.xaml
/// </summary>
public partial class IconButton : UserControl
{
    private const string IconDisabledKey = "disabled";
    private const string IconHighlightedKey = "highlighted";
    private const string IconRegularKey = "regular";

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(
            nameof(Command),
            typeof(ICommand),
            typeof(IconButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(IconButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(IconButton),
            new PropertyMetadata(16.0)); // TODO: Get regular size here

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(IconButton),
            new PropertyMetadata(null));

    public IconButton()
    {
        InitializeComponent();
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public bool HasText => !string.IsNullOrEmpty(Text);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public string State
    {
        get
        {
            if (!IsEnabled)
            {
                return IconDisabledKey;
            }

            if (IsMouseOver)
            {
                return IconHighlightedKey;
            }

            return IconRegularKey;
        }
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}