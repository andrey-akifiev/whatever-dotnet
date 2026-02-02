using System.Windows;
using System.Windows.Controls;

using WhateverDotNet.TestApp.Components.Controls.Models;
using WhateverDotNet.TestApp.Components.Controls.ViewModels;

namespace WhateverDotNet.TestApp.Components.Controls.Views;

/// <summary>
/// Interaction logic for MultiSelectComboBox.xaml
/// </summary>
public partial class MultiSelectComboBox : UserControl
{
    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable<SelectableItem>),
            typeof(MultiSelectComboBox),
            new PropertyMetadata(null, OnItemsChanged));

    private string? _displayText;

    private bool _hasWarning;
    private string? _warningContent;

    private MultiSelectComboBoxViewModel _viewModel;

    public MultiSelectComboBox()
    {
        InitializeComponent();
        _viewModel = new MultiSelectComboBoxViewModel(this);
        DataContext = _viewModel;
    }

    public string? DisplayText
    {
        get => _displayText;
        set => _displayText = value;
    }

    public bool HasWarning
    {
        get => _hasWarning;
        set => _hasWarning = value;
    }

    public IEnumerable<SelectableItem>? Items
    {
        get => (IEnumerable<SelectableItem>?)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
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

    public static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MultiSelectComboBox control)
        {
            return;
        }

        control._viewModel.OnItemsChanged();
    }
}