using CommunityToolkit.Mvvm.Input;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Interaction logic for BrowseItemInput.xaml
/// </summary>
public partial class BrowseItemInput : UserControl
{
    public static readonly DependencyProperty BrowseCommandProperty =
        DependencyProperty.Register(
            nameof(BrowseCommand),
            typeof(RelayCommand),
            typeof(BrowseItemInput));

    public static readonly DependencyProperty CopyToClipboardEnabledProperty =
        DependencyProperty.Register(
            nameof(CopyToClipboardEnabled),
            typeof(bool),
            typeof(BrowseItemInput),
            new FrameworkPropertyMetadata(defaultValue: true));

    public static readonly DependencyProperty CopyToClipboardTemplateProperty =
        DependencyProperty.Register(
            nameof(CopyToClipboardTemplate),
            typeof(string),
            typeof(BrowseItemInput),
            new FrameworkPropertyMetadata(defaultValue: null));

    public static readonly DependencyProperty PathProperty =
        DependencyProperty.Register(
            nameof(Path),
            typeof(string),
            typeof(BrowseItemInput),
            new FrameworkPropertyMetadata(defaultValue: null));

    public static readonly DependencyProperty TooltipProperty =
        DependencyProperty.Register(
            nameof(Tooltip),
            typeof(string),
            typeof(BrowseItemInput),
            new FrameworkPropertyMetadata(defaultValue: string.Empty));

    private bool _hasWarning;
    private string? _warningContent;

    public BrowseItemInput()
    {
        InitializeComponent();
    }

    public ICommand? BrowseCommand
    {
        get => (ICommand?)GetValue(BrowseCommandProperty);
        set => SetValue(BrowseCommandProperty, value);
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

    public string Path
    {
        get => (string)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    public string Tooltip
    {
        get => (string)GetValue(TooltipProperty);
        set => SetValue(TooltipProperty, value);
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
}