using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.Pages;
using WhateverDotNet.TestApp.Components.ViewModels;
using WhateverDotNet.TestApp.Excel.Pages.ReportSpecifications;

namespace WhateverDotNet.TestApp.Excel.Stores;

public class NavigationStore
{
    private readonly  ObservableCollection<PageModel> _pages;

    private BaseViewModel? _currentPage;

    public event Action? CurrentPageChanged;

    public NavigationStore(ReportSpecificationsStore_New reportSpecificationsStore)
    {
        _pages = new ObservableCollection<PageModel>
        {
            new PageModel
            {
                Title = "Specifications",
                Description = "The home page.",
                Icon = "home",
                ViewFactory = () => new ReportSpecificationsViewModel(reportSpecificationsStore),
            },
        };
    }

    public BaseViewModel? CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage == value)
            {
                return;
            }

            _currentPage?.Dispose();
            _currentPage = value;
            OnCurrentPageChanged();
        }
    }

    public ObservableCollection<PageModel> Pages => _pages;

    private void OnCurrentPageChanged()
    {
        CurrentPageChanged?.Invoke();
    }
}