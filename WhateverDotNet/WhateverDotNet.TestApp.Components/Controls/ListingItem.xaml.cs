using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Interaction logic for ListingItem.xaml
/// </summary>
public partial class ListingItem : UserControl
{
    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(ListingItem),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ErrorMessageProperty =
        DependencyProperty.Register(
            nameof(ErrorMessage),
            typeof(string),
            typeof(ListingItem),
            new PropertyMetadata(null));

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(ListingItem),
            new PropertyMetadata(string.Empty));

    public ListingItem()
    {
        InitializeComponent();
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string ErrorMessage
    {
        get => (string)GetValue(ErrorMessageProperty);
        set => SetValue(ErrorMessageProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}