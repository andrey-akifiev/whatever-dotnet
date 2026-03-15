using WhateverDotNet.TestApp.Components.Stores;
using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Controls;

public class HamburgerMenuItemViewModel<TModel>
    : BaseItemViewModel<TModel>
        where TModel : class, ICloneable, IHamburgerMenuItemModel, IModel
{
    private bool _isSelected;

    public HamburgerMenuItemViewModel(TModel model)
        : base(model)
    {
    }

    public string? Description => Model.Description;

    public string? Icon => Model.Icon;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public string? Title => Model.Title;
}