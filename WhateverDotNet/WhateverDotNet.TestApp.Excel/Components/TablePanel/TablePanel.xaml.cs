using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Excel.Components.TablePanel;

/// <summary>
/// Interaction logic for TablePanel.xaml
/// </summary>
public partial class TablePanel : UserControl
{
    public TablePanel()
    {
        InitializeComponent();
        Columns = new ObservableCollection<TableColumn>();
        Loaded += (_, __) => RecalculateColumns();
        SizeChanged += (_, __) => RecalculateColumns();
    }

    public ObservableCollection<TableColumn> Columns { get; }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable),
            typeof(TablePanel));

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object),
            typeof(TablePanel));

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty RowTemplateProperty =
        DependencyProperty.Register(nameof(RowTemplate), typeof(DataTemplate),
            typeof(TablePanel));

    public DataTemplate RowTemplate
    {
        get => (DataTemplate)GetValue(RowTemplateProperty);
        set => SetValue(RowTemplateProperty, value);
    }

    private void RecalculateColumns()
    {
        if (ActualWidth <= 0 || Columns.Count == 0)
            return;

        double available = ActualWidth;
        bool unbounded = double.IsInfinity(available);

        double fixedWidth = Columns
            .Where(c => c.WidthType == TableColumnWidthType.Fixed)
            .Sum(c => c.Width);

        double autoWidth = Columns
            .Where(c => c.WidthType == TableColumnWidthType.Auto)
            .Sum(c => c.MinWidth);

        double remaining = unbounded
            ? 0
            : available - fixedWidth - autoWidth;

        double totalStar = Columns
            .Where(c => c.WidthType == TableColumnWidthType.Star)
            .Sum(c => c.Width);

        foreach (var col in Columns)
        {
            double w = col.WidthType switch
            {
                TableColumnWidthType.Fixed => col.Width,
                TableColumnWidthType.Auto => col.MinWidth,
                TableColumnWidthType.Star when !unbounded =>
                    remaining * (col.Width / totalStar),
                _ => col.MinWidth
            };

            col.Width = w < col.MinWidth ? col.MinWidth : w;
        }
    }
}