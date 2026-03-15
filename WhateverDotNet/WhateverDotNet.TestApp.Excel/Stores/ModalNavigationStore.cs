using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Excel.Stores;

public class ModalNavigationStore
{
    private BaseViewModel? _currentViewModel;

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            if (_currentViewModel == value)
            {
                return;
            }

            _currentViewModel = value;
            CurrentModalViewModelChanged?.Invoke();
        }
    }

    public bool IsOpen => CurrentViewModel != null;

    public event Action? CurrentModalViewModelChanged;

    public void Close()
    {
        CurrentViewModel = null;
    }
}