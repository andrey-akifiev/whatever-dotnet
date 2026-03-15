using WhateverDotNet.TestApp.Components.Navigation;
using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Excel.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly INavigationStore _navigationStore;

    public MainViewModel(
        INavigationStore navigationStore,
        NavigationMenuViewModel menuViewModel)
    {
        _navigationStore = navigationStore;

        MenuViewModel = menuViewModel ?? throw new ArgumentNullException(nameof(menuViewModel));
        _navigationStore.CurrentPageChanged += OnCurrentViewModelChanged;
    }

    public string Logo { get; private set; }

    public NavigationMenuViewModel MenuViewModel { get; }

    public BaseViewModel? CurrentViewModel => _navigationStore.CurrentViewModel;

    public bool IsNavigationCollapsed { get; set; } = false;

    private void OnCurrentViewModelChanged()
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }

    public override void Dispose()
    {
        _navigationStore.CurrentPageChanged -= OnCurrentViewModelChanged;

        base.Dispose();
    }
}