using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using WhateverDotNet.TestApp.Components.Models;

namespace WhateverDotNet.TestApp.Components.Controls;

/// <summary>
/// View model for the listing panel: expand/collapse toggle, Add button, and list of items.
/// Binds list selection to the store's <see cref="IEnlistableItemsStore{TModel}.SelectedItem"/> so a details view can react.
/// </summary>
public partial class ListingPanelViewModel<TModel>
    : ObservableObject, IHasMenuWidth
        where TModel : class, ICloneable, IEnlistableModel, ISampleProvider<TModel>
{
    public const double DefaultCollapsedWidth = 56;
    public const double DefaultExpandedWidth = 260;

    /// <summary>
    /// When true, the list area is visible; when false, only the toggle strip is shown.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MenuWidth))]
    private bool _isExpanded = true;

    [ObservableProperty]
    private ListingPanelItemViewModel<TModel>? _selectedItemViewModel;

    private readonly IEnlistableItemsStore<TModel> _store;

    public ListingPanelViewModel(IEnlistableItemsStore<TModel> store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));

        ItemViewModels = new ObservableCollection<ListingPanelItemViewModel<TModel>>();

        AddCommand = new RelayCommand(() => _store.Add(TModel.CreateSample()));
        ToggleExpandedCommand = new RelayCommand(ToggleExpanded);

        _store.SelectedItemChanged += OnStoreSelectedItemChanged;
        ((INotifyCollectionChanged)_store.Items).CollectionChanged += OnStoreItemsChanged;

        SyncItemViewModelsFromStore();
        SyncSelectedItemViewModelFromStore();
    }

    public ICommand AddCommand { get; }

    public ICommand ToggleExpandedCommand { get; }

    public ObservableCollection<ListingPanelItemViewModel<TModel>> ItemViewModels { get; }

    /// <summary>
    /// Suggested width when expanded (300) or collapsed (56). Bind panel width to this if desired.
    /// </summary>
    public double MenuWidth => IsExpanded
        ? DefaultExpandedWidth
        : DefaultCollapsedWidth;

    /// <summary>
    /// The store's currently selected item (same as selected in the list). Exposed for binding in details view.
    /// </summary>
    public TModel? SelectedItem => _store.SelectedItem;

    public IEnlistableItemsStore<TModel> Store => _store;

    partial void OnSelectedItemViewModelChanged(ListingPanelItemViewModel<TModel>? value)
    {
        if (value?.Model is not TModel model)
        {
            return;
        }

        _store.SelectItem(model);
    }

    private void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
    }

    private void OnStoreItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncItemViewModelsFromStore();
        SyncSelectedItemViewModelFromStore();
    }

    private void OnStoreSelectedItemChanged()
    {
        SyncSelectedItemViewModelFromStore();
        OnPropertyChanged(nameof(SelectedItem));
    }

    private void SyncItemViewModelsFromStore()
    {
        ItemViewModels.Clear();
        foreach (var model in _store.Items)
        {
            ItemViewModels.Add(new ListingPanelItemViewModel<TModel>(model, _store));
        }
    }

    private void SyncSelectedItemViewModelFromStore()
    {
        var selected = _store.SelectedItem;
        var vm = selected == null
            ? null
            : ItemViewModels.FirstOrDefault(ivm => ReferenceEquals(ivm.Model, selected));
        SelectedItemViewModel = vm;
    }
}