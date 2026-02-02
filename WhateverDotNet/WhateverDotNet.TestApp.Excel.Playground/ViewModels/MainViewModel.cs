using WhateverDotNet.TestApp.Excel.ViewModels;

namespace WhateverDotNet.TestApp.Excel.Playground.ViewModels;

public class MainViewModel
{
    public ReportSpecificationsViewModel ReportSpecificationsViewModel { get; }

    public MainViewModel(ReportSpecificationsViewModel reportSpecificationsViewModel)
    {
        ReportSpecificationsViewModel = reportSpecificationsViewModel
            ?? throw new ArgumentNullException(nameof(reportSpecificationsViewModel));
    }
}