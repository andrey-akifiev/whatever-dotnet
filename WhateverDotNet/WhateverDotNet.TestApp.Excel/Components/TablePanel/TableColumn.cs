using System.Windows;

namespace WhateverDotNet.TestApp.Excel.Components.TablePanel;

public class TableColumn
{
    public TableColumnWidthType WidthType { get; set; }
    public double Width { get; set; } = 1; // Fixed px or Star weight
    public double MinWidth { get; set; } = 0;

    public object? Header { get; set; }
}