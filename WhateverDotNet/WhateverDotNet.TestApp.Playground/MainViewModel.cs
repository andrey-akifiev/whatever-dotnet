using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.ViewModels;

namespace WhateverDotNet.TestApp.Playground;

public class MainViewModel : BaseViewModel
{
    public ReportSpecificationsViewModel ReportSpecificationsViewModel { get; }

    public MainViewModel(ReportSpecificationsViewModel reportSpecificationsViewModel)
    {
        ReportSpecificationsViewModel = reportSpecificationsViewModel
            ?? throw new ArgumentNullException(nameof(reportSpecificationsViewModel));
    }
}