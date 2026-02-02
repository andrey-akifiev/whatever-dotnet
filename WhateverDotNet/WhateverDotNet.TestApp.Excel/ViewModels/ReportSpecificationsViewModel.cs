using System.Windows.Input;

using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Models;
using WhateverDotNet.TestApp.Excel.Stores;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class ReportSpecificationsViewModel
    : BaseListingDetailsViewModel<
        ReportSpecificationModel,
        ReportSpecificationsListingItemViewModel_New,
        ReportSpecificationsDetailsViewModel>
{
    private string? _errorMessage;
    private bool _isLoading;

    public ReportSpecificationsViewModel(ReportSpecificationsStore_New store)
        : base(store)
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