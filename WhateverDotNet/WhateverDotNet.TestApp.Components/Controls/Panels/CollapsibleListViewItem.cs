using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

public class CollapsibleListViewItem : ListViewItem
{
    public static DependencyProperty DescriptionProperty =
        DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(CollapsibleListViewItem),
            new PropertyMetadata(null));

    public static DependencyProperty IconProperty =
        DependencyProperty.Register(
            nameof(Icon),
            typeof(string),
            typeof(CollapsibleListViewItem),
            new PropertyMetadata(null));

    public static DependencyProperty IsCollapsedProperty =
        DependencyProperty.Register(
            nameof(IsCollapsed),
            typeof(bool),
            typeof(CollapsibleListViewItem),
            new PropertyMetadata(false));

    public static DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(CollapsibleListViewItem),
            new PropertyMetadata(null));

    static CollapsibleListViewItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CollapsibleListViewItem),
            new FrameworkPropertyMetadata(typeof(CollapsibleListViewItem)));
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool IsCollapsed
    {
        get => (bool)GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}