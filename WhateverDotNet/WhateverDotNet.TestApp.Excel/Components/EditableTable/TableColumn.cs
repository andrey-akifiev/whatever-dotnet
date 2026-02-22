using WhateverDotNet.TestApp.Excel.Components.TablePanel;

namespace WhateverDotNet.TestApp.Excel.Components.EditableTable;

public class TableColumn
{
    public string? Header { get; set; }
    public double MinWidth { get; set; } = 0;
    public double Width { get; set; } = 1; // Fixed px or Star weight
    public TableColumnWidthType WidthType { get; set; }
}