using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.Controls;
using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationColumnsDetailsViewModel
    : RevertibleCollectionViewModel<ReportSpecificationColumnDetailsViewModel, ReportSpecificationColumnModel>
{
    public ReportSpecificationColumnsDetailsViewModel()
        : base(new ObservableCollection<ReportSpecificationColumnDetailsViewModel>())
    {
    }

    public ObservableCollection<TableColumn> Columns { get; } = new ObservableCollection<TableColumn>
    {
        new TableColumn
        {
            Name = "Title",
        },
        new TableColumn
        {
            Name = "DisplayName",
            DisplayName = "Display Title",
        },
        new TableColumn
        {
            Name = "Type",
        },
        new TableColumn
        {
            Name = "Format",
        },
        new TableColumn
        {
            Name = "IsComparable",
            DisplayName = "Comparable",
            Width = 80, // TODO: Implement support -- aa
        },
        new TableColumn
        {
            Name = "IsValuable",
            DisplayName = "Valuable",
            Width = 80,
        },
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