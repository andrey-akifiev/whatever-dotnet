using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace WhateverDotNet.TestApp.Components.Controls;

public partial class EditableTable : UserControl
{
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register(
            nameof(Columns),
            typeof(ObservableCollection<TableColumn>),
            typeof(EditableTable),
            new PropertyMetadata(
                defaultValue: null,
                propertyChangedCallback: OnColumnsChanged));

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(
            nameof(Items),
            typeof(IEnumerable),
            typeof(EditableTable),
            new PropertyMetadata(null, OnItemsChanged));

    public static readonly DependencyProperty ColumnTemplatesProperty =
        DependencyProperty.Register(
            nameof(ColumnTemplates),
            typeof(ObservableCollection<TableColumnTemplate>),
            typeof(EditableTable),
            new PropertyMetadata(new ObservableCollection<TableColumnTemplate>()));

    private static readonly DependencyPropertyKey HasItemsPropertyKey =
        DependencyProperty.RegisterReadOnly(
            nameof(HasItems),
            typeof(bool),
            typeof(EditableTable),
            new PropertyMetadata(false));

    public static readonly DependencyProperty HasItemsProperty = HasItemsPropertyKey.DependencyProperty;

    public EditableTable()
    {
        InitializeComponent();
        SizeChanged += OnSizeChanged;
    }

    public ObservableCollection<TableColumn>? Columns
    {
        get => GetValue(ColumnsProperty) as ObservableCollection<TableColumn>;
        set => SetValue(ColumnsProperty, value);
    }

    /// <summary>
    /// Items to display. You can pass a `RevertibleCollectionViewModel&lt;,&gt;` instance directly
    /// because it implements `IEnumerable` over its `Current` collection.
    /// </summary>
    public IEnumerable? Items
    {
        get => GetValue(ItemsProperty) as IEnumerable;
        set => SetValue(ItemsProperty, value);
    }

    /// <summary>
    /// Optional mapping from column name to a specific cell DataTemplate.
    /// If no template is provided for a column, the default text template is used.
    /// </summary>
    public ObservableCollection<TableColumnTemplate> ColumnTemplates
    {
        get => (ObservableCollection<TableColumnTemplate>)GetValue(ColumnTemplatesProperty);
        set => SetValue(ColumnTemplatesProperty, value);
    }

    /// <summary>
    /// True when the bound Items collection contains at least one item.
    /// </summary>
    public bool HasItems
    {
        get => (bool)GetValue(HasItemsProperty);
        private set => SetValue(HasItemsPropertyKey, value);
    }

    private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (EditableTable)d;

        if (e.OldValue is INotifyCollectionChanged oldChanged)
        {
            oldChanged.CollectionChanged -= control.OnColumnsCollectionChanged;
        }

        if (e.NewValue is INotifyCollectionChanged newChanged)
        {
            newChanged.CollectionChanged += control.OnColumnsCollectionChanged;
        }

        control.RecalculateColumnWidths();
    }

    private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (EditableTable)d;

        if (e.OldValue is INotifyCollectionChanged oldChanged)
        {
            oldChanged.CollectionChanged -= control.OnItemsCollectionChanged;
        }

        if (e.NewValue is INotifyCollectionChanged newChanged)
        {
            newChanged.CollectionChanged += control.OnItemsCollectionChanged;
        }

        control.UpdateHasItems();
    }

    private void OnColumnsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RecalculateColumnWidths();
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateHasItems();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged)
        {
            RecalculateColumnWidths();
        }
    }

    private void RecalculateColumnWidths()
    {
        if (Columns == null || Columns.Count == 0)
        {
            return;
        }

        // Total width available for the control.
        double totalWidth = ActualWidth;
        if (double.IsNaN(totalWidth) || totalWidth <= 0)
        {
            return;
        }

        // Reserve space for the actions column (approximate fixed width).
        const double actionsColumnWidth = 80.0;
        double availableForColumns = totalWidth - actionsColumnWidth;
        if (availableForColumns <= 0)
        {
            availableForColumns = totalWidth;
        }

        double perColumnWidth = availableForColumns / Columns.Count;

        foreach (var column in Columns)
        {
            column.Width = perColumnWidth;
        }
    }

    private void UpdateHasItems()
    {
        var items = Items;
        if (items == null)
        {
            HasItems = false;
            return;
        }

        if (items is ICollection collection)
        {
            HasItems = collection.Count > 0;
            return;
        }

        var enumerator = items.GetEnumerator();
        try
        {
            HasItems = enumerator.MoveNext();
        }
        finally
        {
            (enumerator as IDisposable)?.Dispose();
        }
    }
}