using CommunityToolkit.Mvvm.Input;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.Controls;

public class BaseComboBox : ComboBox
{
    public static readonly DependencyProperty CopyToClipboardFormatProperty =
        DependencyProperty.Register(
            nameof(CopyToClipboardFormat),
            typeof(string),
            typeof(BaseComboBox),
            new PropertyMetadata(null));

    public static readonly DependencyProperty OriginalValueProperty =
        DependencyProperty.Register(
            nameof(OriginalValue),
            typeof(object),
            typeof(BaseComboBox),
            new PropertyMetadata(null));

    public static readonly DependencyProperty PlaceholderValueProperty =
        DependencyProperty.Register(
            nameof(PlaceholderValue),
            typeof(string),
            typeof(BaseComboBox),
            new PropertyMetadata(null));

    public static readonly DependencyProperty ShowClearProperty =
        DependencyProperty.Register(
            nameof(ShowClear),
            typeof(bool),
            typeof(BaseComboBox),
            new PropertyMetadata(false));

    public static readonly DependencyProperty ShowCopyProperty =
        DependencyProperty.Register(
            nameof(ShowCopy),
            typeof(bool),
            typeof(BaseComboBox),
            new PropertyMetadata(false));

    public static readonly DependencyProperty ShowPlaceholderProperty =
        DependencyProperty.Register(
            nameof(ShowPlaceholder),
            typeof(bool),
            typeof(BaseComboBox),
            new PropertyMetadata(true));

    public static readonly DependencyProperty ShowRevertProperty =
        DependencyProperty.Register(
            nameof(ShowRevert),
            typeof(bool),
            typeof(BaseComboBox),
            new PropertyMetadata(false));

    public static readonly DependencyProperty ContentMarginProperty =
        DependencyProperty.Register(
            nameof(ContentMargin),
            typeof(Thickness),
            typeof(BaseComboBox),
            new PropertyMetadata(default(Thickness)));

    static BaseComboBox()
    {
        DefaultStyleKeyProperty
            .OverrideMetadata(
                typeof(BaseComboBox),
                new FrameworkPropertyMetadata(typeof(BaseComboBox)));
    }

    public BaseComboBox()
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

    public object? OriginalValue
    {
        get => GetValue(OriginalValueProperty);
        set => SetValue(OriginalValueProperty, value);
    }

    public string? PlaceholderValue
    {
        get => (string?)GetValue(PlaceholderValueProperty);
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

    public Thickness ContentMargin
    {
        get => (Thickness)GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        base.OnSelectionChanged(e);
        NotifyCommandCanExecuteChanged();
    }

    private bool CanClearValue() => SelectedItem != null;

    private bool CanRevertValue()
    {
        var original = OriginalValue;
        if (original == null)
        {
            return SelectedItem != null;
        }

        return !Equals(original, SelectedItem);
    }

    private void ClearValue() => SelectedItem = null;

    private void RevertValue() => SelectedItem = OriginalValue;

    private void NotifyCommandCanExecuteChanged()
    {
        (ClearValueCommand as RelayCommand)?.NotifyCanExecuteChanged();
        (RevertValueCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }
}
