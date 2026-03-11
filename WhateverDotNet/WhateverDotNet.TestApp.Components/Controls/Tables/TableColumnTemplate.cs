using System.Windows;

namespace WhateverDotNet.TestApp.Components.Controls;

public class TableColumnTemplate : DependencyObject
{
    public static readonly DependencyProperty ColumnNameProperty =
        DependencyProperty.Register(
            nameof(ColumnName),
            typeof(string),
            typeof(TableColumnTemplate),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty CellTemplateProperty =
        DependencyProperty.Register(
            nameof(CellTemplate),
            typeof(DataTemplate),
            typeof(TableColumnTemplate),
            new PropertyMetadata(null));

    public string ColumnName
    {
        get => (string)GetValue(ColumnNameProperty);
        set => SetValue(ColumnNameProperty, value);
    }

    public DataTemplate? CellTemplate
    {
        get => GetValue(CellTemplateProperty) as DataTemplate;
        set => SetValue(CellTemplateProperty, value);
    }
}