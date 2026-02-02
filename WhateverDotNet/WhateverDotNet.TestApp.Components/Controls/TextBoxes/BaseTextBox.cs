using CommunityToolkit.Mvvm.Input;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.Controls;

public class BaseTextBox : TextBox
{
    public static readonly DependencyProperty CopyToClipboardFormatProperty =
        DependencyProperty.Register(
            nameof(CopyToClipboardFormat),
            typeof(string),
            typeof(BaseTextBox),
            new PropertyMetadata(null));

    public static readonly DependencyProperty OriginalValueProperty =
        DependencyProperty.Register(
            nameof(OriginalValue),
            typeof(string),
            typeof(BaseTextBox),
            new PropertyMetadata(null));

    public static readonly DependencyProperty PlaceholderValueProperty =
        DependencyProperty.Register(
            nameof(PlaceholderValue),
            typeof(string),
            typeof(BaseTextBox),
            new PropertyMetadata(null));

    public static readonly DependencyProperty ShowClearProperty =
        DependencyProperty.Register(
            nameof(ShowClear),
            typeof(bool),
            typeof(BaseTextBox),
            new PropertyMetadata(false));

    public static readonly DependencyProperty ShowCopyProperty =
        DependencyProperty.Register(
            nameof(ShowCopy),
            typeof(bool),
            typeof(BaseTextBox),
            new PropertyMetadata(false));

    public static readonly DependencyProperty ShowPlaceholderProperty =
        DependencyProperty.Register(
            nameof(ShowPlaceholder),
            typeof(bool),
            typeof(BaseTextBox),
            new PropertyMetadata(true));

    public static readonly DependencyProperty ShowRevertProperty =
        DependencyProperty.Register(
            nameof(ShowRevert),
            typeof(bool),
            typeof(BaseTextBox),
            new PropertyMetadata(false));

    public static readonly DependencyProperty TextMarginProperty =
        DependencyProperty.Register(
            nameof(TextMargin),
            typeof(Thickness),
            typeof(BaseTextBox),
            new PropertyMetadata(default(Thickness)));

    static BaseTextBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(BaseTextBox),
                new FrameworkPropertyMetadata(typeof(BaseTextBox)));
    }

    public BaseTextBox()
    {
        ClearValueCommand = new RelayCommand(
            execute: ClearValue,
            canExecute: CanClearValue);
        RevertValueCommand = new RelayCommand(
            execute: RevertValue,
            canExecute: CanRevertValue);
    }

    public ICommand ClearValueCommand { get; }

    public ICommand RevertValueCommand { get; }

    public string? CopyToClipboardFormat
    {
        get => (string?)GetValue(CopyToClipboardFormatProperty);
        set => SetValue(CopyToClipboardFormatProperty, value);
    }

    public string? OriginalValue
    {
        get => (string)GetValue(OriginalValueProperty);
        set => SetValue(OriginalValueProperty, value);
    }

    public string? PlaceholderValue
    {
        get => (string)GetValue(PlaceholderValueProperty);
        set => SetValue(PlaceholderValueProperty, value);
    }

    public bool ShowClear
    {
        get => (bool)GetValue(ShowClearProperty);
        set => SetValue(ShowClearProperty, value);
    }

    public bool ShowCopy
    {
        get => (bool)GetValue(ShowCopyProperty);
        set => SetValue(ShowCopyProperty, value);
    }

    public bool ShowPlaceholder
    {
        get => (bool)GetValue(ShowPlaceholderProperty);
        set => SetValue(ShowPlaceholderProperty, value);
    }

    public bool ShowRevert
    {
        get => (bool)GetValue(ShowRevertProperty);
        set => SetValue(ShowRevertProperty, value);
    }

    public Thickness TextMargin
    {
        get => (Thickness)GetValue(TextMarginProperty);
        set => SetValue(TextMarginProperty, value);
    }

    private bool CanClearValue() => !string.IsNullOrEmpty(Text);

    private bool CanRevertValue() => OriginalValue != null && OriginalValue != Text;

    private void ClearValue() => Text = string.Empty;

    private void RevertValue() => Text = OriginalValue;
}