using WhateverDotNet.TestApp.Components.Controls;
using WhateverDotNet.TestApp.Components.Models;
using WhateverDotNet.TestApp.Components.Stores;
using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Pages;

public abstract class BaseListingPageViewModel<TPageModel, TItemModel, TListingItemViewModel, TDetailsViewModel>
    : BasePageViewModel<TPageModel>
        where TPageModel : PageModel
        where TItemModel : class, ICloneable, IEnlistableModel, IModel, ISampleProvider<TItemModel>
        where TListingItemViewModel : class, IItemViewModel<TItemModel>
        where TDetailsViewModel : class
{
    protected readonly IEnlistableItemsStore<TItemModel> _store;

    private TDetailsViewModel? _detailsViewModel;

    public BaseListingPageViewModel(IEnlistableItemsStore<TItemModel> store, TPageModel model)
        : base(model)
    {
        _store = store
            ?? throw new ArgumentNullException(nameof(store));

        // Create listing items from store
        Listing = new ListingPanelViewModel<TItemModel>(_store);

        // Subscribe to store events if any
        if (store is IModelStoreEvents<TItemModel> storeEvents)
        {
            store.SelectedItemChanged += OnSelectedChanged;
        }
    }

    private void OnSelectedChanged()
    {
        TItemModel? selectedItem = _store.SelectedItem;

        if (selectedItem is null) return;

        Details = CreateDetailsViewModel(_store.SelectedItem!);
    }

    /// <summary>
    /// Gets the view model that manages details of a particular item.
    /// </summary>
    public TDetailsViewModel? Details
    {
        get => _detailsViewModel;
        private set
        {
            _detailsViewModel = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the view model that manages the collection of listing items and their associated data model.
    /// <para>Responsible for listing and selection.</para>
    /// </summary>
    public ListingPanelViewModel<TItemModel> Listing { get; }

    public override void Dispose()
    {
        
        if (_store is IModelStoreEvents<TItemModel> storeEvents)
        {
        }

        DisposeDetailsViewModel(_detailsViewModel);
        _detailsViewModel = null;

        base.Dispose();
    }

    protected abstract TDetailsViewModel CreateDetailsViewModel(TItemModel model);

    protected abstract TListingItemViewModel CreateListingItemViewModel(TItemModel model);

    protected virtual void DisposeDetailsViewModel(TDetailsViewModel? detailsViewModel)
    {
        if (detailsViewModel is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}