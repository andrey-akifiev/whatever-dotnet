using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationsListingItemViewModel : BaseViewModel
{
    public ReportSpecificationsListingItemViewModel(ReportSpecificationModel report)
    {
        Report = report
            ?? throw new ArgumentNullException(nameof(report));
    }

    public string Description => Report.Description ?? "Unknown";

    public string Title => Report.Name ?? "Unknown";

    public ReportSpecificationModel Report { get; private set; }

    public void Update(ReportSpecificationModel report)
    {
        Report = report;
        OnPropertyChanged(); // TODO:
    }
}