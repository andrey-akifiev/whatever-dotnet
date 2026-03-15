using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationsListingItemViewModel_New
    : BaseViewModel, IItemViewModel<ReportSpecificationModel>
{
    public ReportSpecificationsListingItemViewModel_New(ReportSpecificationModel model)
    {
        Model = model;
    }

    public string Description => Model.Description ?? string.Empty;

    public Guid Id { get => Model.Id; }

    public ReportSpecificationModel Model { get; private set; }

    public string Name => Model.Name ?? "UNKNOWN";

    public ReportSpecificationModel CloneModel(ReportSpecificationModel source)
        => source.Clone() as ReportSpecificationModel
            ?? throw new InvalidOperationException("Clone returned null or an object of the wrong type.");

    public void Update(ReportSpecificationModel model)
    {
        Model = model;
        OnPropertyChanged(nameof(Description));
        OnPropertyChanged(nameof(Name));
    }
}