using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Components.TablePanel;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationColumnsDetailsViewModel
    : BaseGridViewModel<ReportSpecificationColumnDetailsViewModel, ReportSpecificationColumnModel>
{
    public ReportSpecificationColumnsDetailsViewModel()
        : base(new ObservableCollection<ReportSpecificationColumnDetailsViewModel>())
    {
    }
    public ObservableCollection<RowVm> Rows { get; } =
            new ObservableCollection<RowVm>
            {
                new() { A = "One", B = "Alpha" },
                new() { A = "Two", B = "Beta" },
                new() { A = "Three", B = "Gamma" }
            };
    public void AddRange(IEnumerable<ReportSpecificationColumnModel> items)
    {
        foreach (var item in items)
        {
            this.Add(CreateNewItem(item));
        }
    }

    protected override ReportSpecificationColumnDetailsViewModel CreateNewItem(ReportSpecificationColumnModel model)
    {
        return new ReportSpecificationColumnDetailsViewModel(model);
    }
}