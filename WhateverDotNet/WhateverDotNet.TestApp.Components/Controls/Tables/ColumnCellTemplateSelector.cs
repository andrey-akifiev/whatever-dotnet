using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// Selects a cell template based on the current column (TableColumn)
/// while the item passed in is the current row view model.
/// </summary>
public sealed class ColumnCellTemplateSelector : DataTemplateSelector
{
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (container is not FrameworkElement element)
        {
            return base.SelectTemplate(item, container);
        }

        // Walk up the visual tree until we find a FrameworkElement whose DataContext is a TableColumn.
        var column = FindColumn(element);
        if (column == null || string.IsNullOrWhiteSpace(column.Name))
        {
            return base.SelectTemplate(item, container);
        }

        var table = FindAncestor<EditableTable>(element);
        var templates = table?.ColumnTemplates;
        if (templates == null)
        {
            return base.SelectTemplate(item, container);
        }

        var match = templates.FirstOrDefault(t =>
            string.Equals(t.ColumnName, column.Name, StringComparison.Ordinal));

        return match?.CellTemplate ?? base.SelectTemplate(item, container);
    }

    private static TableColumn? FindColumn(FrameworkElement start)
    {
        DependencyObject? current = start;

        while (current != null)
        {
            if (current is FrameworkElement fe && fe.DataContext is TableColumn column)
            {
                return column;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private static T? FindAncestor<T>(DependencyObject? current)
        where T : DependencyObject
    {
        while (current != null)
        {
            if (current is T typed)
            {
                return typed;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }
}