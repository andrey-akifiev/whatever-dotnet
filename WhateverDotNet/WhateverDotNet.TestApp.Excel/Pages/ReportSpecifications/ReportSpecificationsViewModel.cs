using System.Windows.Input;

using WhateverDotNet.TestApp.Components.Pages;
using WhateverDotNet.TestApp.Excel.Models;
using WhateverDotNet.TestApp.Excel.Stores;
using WhateverDotNet.TestApp.Excel.ViewModels;

namespace WhateverDotNet.TestApp.Excel.Pages.ReportSpecifications;

public class ReportSpecificationsViewModel
    : BaseListingPageViewModel<
        ReportSpecificationsPageModel,
        ReportSpecificationModel,
        ReportSpecificationsListingItemViewModel_New,
        ReportSpecificationsDetailsViewModel>
{
    private string? _errorMessage;
    private bool _isLoading;

    public ReportSpecificationsViewModel(ReportSpecificationsStore_New store)
        : base(store, new ReportSpecificationsPageModel())
    {
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasErrorMessage));
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged(nameof(IsLoading));
        }
    }

    public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

    public ICommand LoadReportsCommand { get; }

    public static ReportSpecificationsViewModel LoadViewModel(ReportSpecificationsStore_New store)
    {
        var viewModel = new ReportSpecificationsViewModel(store);
        _ = store.LoadAsync();
        return viewModel;
    }

    protected override ReportSpecificationsDetailsViewModel CreateDetailsViewModel(ReportSpecificationModel model)
        => new ReportSpecificationsDetailsViewModel(
            model ?? ReportSpecificationModel.CreateSample(),
            (ReportSpecificationsStore_New)_store);

    protected override ReportSpecificationsListingItemViewModel_New CreateListingItemViewModel(ReportSpecificationModel model)
        => new ReportSpecificationsListingItemViewModel_New(model);
}