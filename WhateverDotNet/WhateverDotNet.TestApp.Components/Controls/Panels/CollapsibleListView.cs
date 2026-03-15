using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

public class CollapsibleListView : ListView
{
    public static DependencyProperty CollapsedLogoProperty =
        DependencyProperty.Register(
            nameof(CollapsedLogo),
            typeof(object),
            typeof(CollapsibleListView),
            new PropertyMetadata(null));

    public static DependencyProperty LogoProperty =
        DependencyProperty.Register(
            nameof(Logo),
            typeof(object),
            typeof(CollapsibleListView),
            new PropertyMetadata(null));

    public static DependencyProperty IsCollapsedProperty =
        DependencyProperty.Register(
            nameof(IsCollapsed),
            typeof(bool),
            typeof(CollapsibleListView),
            new PropertyMetadata(false));

    public static DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(CollapsibleListView),
            new PropertyMetadata(null));

    static CollapsibleListView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(CollapsibleListView),
            new FrameworkPropertyMetadata(typeof(CollapsibleListView)));
    }

    public object CollapsedLogo
    {
        get => GetValue(CollapsedLogoProperty);
        set => SetValue(CollapsedLogoProperty, value);
    }

    public bool IsCollapsed
    {
        get => (bool)GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }

    public object Logo
    {
        get
        {
            object logo = GetValue(LogoProperty);

            if (IsCollapsed)
            {
                object collapsedLogo = GetValue(CollapsedLogoProperty);
                return collapsedLogo != null
                    ? collapsedLogo
                    : logo;
            }

            return logo;
        }

        set => SetValue(LogoProperty, value);
    }

    public bool HasLogo => Logo != null;

    public bool HasTitle => !string.IsNullOrEmpty(Title);

    public string? Title
    {
        get => (string?)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new CollapsibleListViewItem();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is CollapsibleListViewItem;
    }
}