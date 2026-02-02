using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationColumnsDetailsViewModel
    : BaseGridViewModel<ReportSpecificationColumnDetailsViewModel, ReportSpecificationColumnModel>
{
    public ReportSpecificationColumnsDetailsViewModel()
        : base(new ObservableCollection<ReportSpecificationColumnDetailsViewModel>())
    {
    }

    public override void AddRange(IEnumerable<ReportSpecificationColumnModel> models)
    {
        foreach (var viewModel in models.Select(CreateNewRow))
        {
            Rows.Add(viewModel);
        }
    }

    protected override ReportSpecificationColumnModel CloneModel(ReportSpecificationColumnModel model)
        => (ReportSpecificationColumnModel)model.Clone();

    protected override ReportSpecificationColumnDetailsViewModel CreateNewRow() =>
        CreateNewRow(ReportSpecificationColumnModel.CreateSample());

    protected override ReportSpecificationColumnDetailsViewModel CreateNewRow(ReportSpecificationColumnModel model)
    {
        return new ReportSpecificationColumnDetailsViewModel(
            model,
            onClone: rvm => CloneRow((ReportSpecificationColumnDetailsViewModel)rvm),
            onDelete: rvm => DeleteRow((ReportSpecificationColumnDetailsViewModel)rvm));
    }
}