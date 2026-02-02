using System.Windows;
using System.Windows.Controls;
using WhateverDotNet.TestApp.Components.Helpers;

namespace WhateverDotNet.TestApp.Excel.Components;

/// <summary>
/// Interaction logic for ReportSpecificationColumnDetails.xaml
/// </summary>
public partial class ReportSpecificationColumnsDetails : UserControl
{
    public ReportSpecificationColumnsDetails()
    {
        InitializeComponent();
    }

    private void DataGrid_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
    {
        DataGridEventHelper.DataGridPreviewMouseLeftButtonDownEvent(sender, e);
    }
}