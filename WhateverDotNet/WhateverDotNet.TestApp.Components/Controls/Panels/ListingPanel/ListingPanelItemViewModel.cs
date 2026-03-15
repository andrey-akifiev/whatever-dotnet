using System.Windows.Input;

using CommunityToolkit.Mvvm.Input;

using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// View model for a single item in the listing panel: title, description, and actions (Clone, Remove).
/// </summary>
public partial class ListingPanelItemViewModel<TModel> : BaseItemViewModel<TModel>
    where TModel : class, ICloneable, IEnlistableModel
{
    private readonly IEnlistableItemsStore<TModel> _store;

    public ListingPanelItemViewModel(TModel model, IEnlistableItemsStore<TModel> store)
        : base(model)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        CloneCommand = new RelayCommand(() => _store.Add((TModel)Model.Clone()));
        RemoveCommand = new RelayCommand(() => _store.Remove(Model));
    }

    public string? Description => Model.Description;

    public string? Title => Model.Title ?? string.Empty;

    public ICommand CloneCommand { get; }

    public ICommand RemoveCommand { get; }
}