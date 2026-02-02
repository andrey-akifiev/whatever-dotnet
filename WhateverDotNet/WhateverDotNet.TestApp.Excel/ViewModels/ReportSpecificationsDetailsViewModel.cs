using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;
using WhateverDotNet.TestApp.Excel.Stores;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationsDetailsViewModel : BaseViewModel, IDisposable
{
    private readonly ReportSpecificationsStore_New _store;

    public ReportSpecificationsDetailsViewModel(ReportSpecificationModel model, ReportSpecificationsStore_New store)
    {
        _store = store;
        Current = model; // TODO: CloneModel -- aa

        if (Current.Worksheets == null)
        {
            Current.Worksheets = Array.Empty<ReportSpecificationWorksheetModel>();
        }

        foreach (var sheet in Current.Worksheets)
        {
            Worksheets.Add(new ReportSpecificationWorksheetViewModel(sheet));
        }

        //Worksheets = new ObservableCollection<ReportSpecificationWorksheetViewModel>(
        //    Current.Worksheets.Select(w => new ReportSpecificationWorksheetViewModel(w)));

        //Worksheets = new ObservableCollection<ReportSpecificationWorksheetViewModel>(
        //    model.Worksheets?.Values?.Select(w => new ReportSpecificationWorksheetViewModel(w))
        //    ?? Enumerable.Empty<ReportSpecificationWorksheetViewModel>());
    }

    public ReportSpecificationModel Current { get; }

    public string? Description
    {
        get => Current.Description;
        set => Current.Description = value;
    }

    public string Title
    {
        get => Current.Name ?? "UNKNOWN";
        set => Current!.Name = value;
    }

    public ObservableCollection<ReportSpecificationWorksheetViewModel> Worksheets { get; } = new ();

    public override void Dispose()
    {
        base.Dispose();
    }
}