using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WhateverDotNet.TestApp.Excel.Components.EditableTable;

/// <summary>
/// Table of components for editing a collection of complex items.
/// For "always ready to edit" behaviour you can:
/// 1) Use a DataGrid with PreviewMouseLeftButtonDown handled by
///    <see cref="WhateverDotNet.TestApp.Components.Helpers.DataGridEventHelper.DataGridPreviewMouseLeftButtonDownEvent"/>,
///    which puts the cell into edit mode on first click; or
/// 2) Use <see cref="WhateverDotNet.TestApp.Excel.Components.TablePanel.TablePanel"/> with a RowTemplate
///    that contains the actual controls (e.g. SimplifiedTextBox, ComboBox) so each row is always editable.
/// </summary>
public partial class EditableTable : UserControl
{
    public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register(
            nameof(Columns),
            typeof(TableColumn[]),
            typeof(EditableTable),
            new PropertyMetadata(Array.Empty<TableColumn>()));


    public EditableTable()
    {
        InitializeComponent();
        BuildColumns();
    }

    public TableColumn[] Columns { get; set; }

    private void BuildColumns()
    {
        var columns = new TableColumn[]
        {
            new TableColumn
            {
                Header = "Name",
                WidthType = TablePanel.TableColumnWidthType.Star,
            },
            new TableColumn
            {
                Header = "Display Name",
                WidthType = TablePanel.TableColumnWidthType.Star,
            },
        };

        for (int idx = 0; idx < columns.Length; idx++)
        {
            tblHeaderHost
                .ColumnDefinitions
                .Add(new ColumnDefinition { Width = new GridLength(150, GridUnitType.Star) });

            TextBlock txtBlock = new()
            {
                Background = new SolidColorBrush(Colors.Red),
                Text = columns[idx].Header,
            };

            Grid.SetColumn(txtBlock, idx);
            Grid.SetRow(txtBlock, 0);

            tblHeaderHost
                .Children
                .Add(txtBlock);
        }
    }
}