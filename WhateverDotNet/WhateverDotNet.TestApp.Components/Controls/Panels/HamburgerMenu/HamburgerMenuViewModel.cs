using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using System.Collections.ObjectModel;
using System.Windows.Input;

using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.Controls;

public partial class HamburgerMenuViewModel<TModel>
    : ObservableObject, IHasMenuWidth
        where TModel : class, ICloneable, IHamburgerMenuItemModel, IModel
{
    public const double DefaultCollapsedWidth = 56;
    public const double DefaultExpandedWidth = 220;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MenuWidth))]
    private bool _isExpanded = true;

    public HamburgerMenuViewModel()
    {
        BottomMenuItems = new ObservableCollection<HamburgerMenuItemViewModel<TModel>>();
        TopMenuItems = new ObservableCollection<HamburgerMenuItemViewModel<TModel>>();
        
        SelectItemCommand = new RelayCommand<HamburgerMenuItemViewModel<TModel>?>(SelectItem);
        ToggleExpandedCommand = new RelayCommand(ToggleExpanded);
    }

    public ObservableCollection<HamburgerMenuItemViewModel<TModel>> BottomMenuItems { get; }
    
    /// <summary>
    /// Suggested width when expanded (220) or collapsed (56). Bind panel width to this if desired.
    /// </summary>
    public double MenuWidth => IsExpanded
        ? DefaultExpandedWidth
        : DefaultCollapsedWidth;

    public ICommand SelectItemCommand { get; }

    public ICommand ToggleExpandedCommand { get; }

    public ObservableCollection<HamburgerMenuItemViewModel<TModel>> TopMenuItems { get; }

    protected virtual void SelectItem(HamburgerMenuItemViewModel<TModel>? item)
    {
        // Default implementation does nothing, override in derived classes to navigate or perform other actions.
    }

    private void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }
}