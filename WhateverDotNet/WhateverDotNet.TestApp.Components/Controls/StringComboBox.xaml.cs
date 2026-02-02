using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Interaction logic for StringComboBox.xaml
/// </summary>
public partial class StringComboBox : UserControl
{
    public static readonly DependencyProperty CopyToClipboardEnabledProperty =
        DependencyProperty.Register(
            nameof(CopyToClipboardEnabled),
            typeof(bool),
            typeof(StringComboBox),
            new FrameworkPropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty CopyToClipboardTemplateProperty =
        DependencyProperty.Register(
            nameof(CopyToClipboardTemplate),
            typeof(string),
            typeof(StringComboBox),
            new FrameworkPropertyMetadata(defaultValue: null));

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<string>),
            typeof(StringComboBox),
            new PropertyMetadata(defaultValue: null));

    public static readonly DependencyProperty SelectedValueProperty =
        DependencyProperty.Register(
            nameof(SelectedValue),
            typeof(string),
            typeof(StringComboBox),
            new FrameworkPropertyMetadata(
                defaultValue: null,
                flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    private bool _hasWarning;
    private string? _warningContent;

    public StringComboBox()
    {
        InitializeComponent();
    }

    public bool CopyToClipboardEnabled
    {
        get => (bool)GetValue(CopyToClipboardEnabledProperty);
        set => SetValue(CopyToClipboardEnabledProperty, value);
    }

    public string CopyToClipboardTemplate
    {
        get => (string)GetValue(CopyToClipboardTemplateProperty);
        set => SetValue(CopyToClipboardTemplateProperty, value);
    }

    public bool HasWarning
    {
        get => _hasWarning;
        set => _hasWarning = value;
    }

    public string? WarningContent
    {
        get => _warningContent;
        set
        {
            _warningContent = value;
            HasWarning = !string.IsNullOrEmpty(value);
        }
    }

    public IEnumerable<string> Items
    {
        get => (IEnumerable<string>)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public string? SelectedValue
    {
        get => (string?)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }
}