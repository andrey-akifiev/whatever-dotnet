using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Excel.Stores;

public class NavigationService<TViewModel>
    where TViewModel : BaseViewModel
{
    private readonly NavigationStore _navigationStore;
    private readonly Func<TViewModel> _viewModelFactory;

    public NavigationService(NavigationStore navigationStore, Func<TViewModel> viewModelFactory)
    {
        _navigationStore = navigationStore
            ?? throw new ArgumentNullException(nameof(navigationStore));
        _viewModelFactory = viewModelFactory
            ?? throw new ArgumentNullException(nameof(viewModelFactory));
    }

    public void Navigate()
    {
        _navigationStore.CurrentPage = _viewModelFactory();
    }
}