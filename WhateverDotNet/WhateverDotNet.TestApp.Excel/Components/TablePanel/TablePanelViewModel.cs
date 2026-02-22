using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WhateverDotNet.TestApp.Excel.Components.TablePanel;

public class RowVm
{
    public string A { get; set; }
    public string B { get; set; }
}

public class TablePanelViewModel
{
    public ObservableCollection<RowVm> Rows { get; } =
            new ObservableCollection<RowVm>
            {
                new() { A = "One", B = "Alpha" },
                new() { A = "Two", B = "Beta" },
                new() { A = "Three", B = "Gamma" }
            };
}
