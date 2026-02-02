using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationsListingItemViewModel_New
    : BaseViewModel, IListingItemViewModel<ReportSpecificationModel>
{
    public ReportSpecificationsListingItemViewModel_New(ReportSpecificationModel model)
    {
        Model = model;
    }

    public string Description => Model.Description ?? string.Empty;

    public Guid Id { get => Model.Id; }

    public ReportSpecificationModel Model { get; private set; }

    public string Name => Model.Name ?? "UNKNOWN";

    public void Update(ReportSpecificationModel model)
    {
        Model = model;
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Name));
    }
}