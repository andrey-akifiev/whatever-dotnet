using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class BaseListingDetailsViewModel<TModel, TListingItemViewModel, TDetailsViewModel> 
    : BaseViewModel
        where TModel : class, IModel
        where TListingItemViewModel : class, IListingItemViewModel<TModel>
        where TDetailsViewModel: class
{
    protected readonly IModelStore<TModel> _store;

    private TDetailsViewModel? _detailsViewModel;

    public BaseListingDetailsViewModel(IModelStore<TModel> store)
    {
        _store = store;

        // Create listing items from store
        Listing = new BaseListingViewModel<TListingItemViewModel, TModel>(
            store.Items.Select(CreateListingItemViewModel));
        
        Listing?.SelectedChanged += OnSelectedChanged;

        // Subscribe to store events if any
        if (store is IModelStoreEvents<TModel> storeEvents)
        {
            storeEvents.Added += OnModelAdded;
            storeEvents.Loaded += OnModelsLoaded;
            storeEvents.Removed += OnModelRemoved;
            storeEvents.Updated += OnModelUpdated;
        }
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
    public BaseListingViewModel<TListingItemViewModel, TModel> Listing { get; }

    public override void Dispose()
    {
        Listing?.SelectedChanged -= OnSelectedChanged;

        if (_store is IModelStoreEvents<TModel> storeEvents)
        {
            storeEvents?.Added -= OnModelAdded;
            storeEvents?.Removed -= OnModelRemoved;
            storeEvents?.Updated -= OnModelUpdated;
        }

        DisposeDetailsViewModel(_detailsViewModel);
        _detailsViewModel = null;

        base.Dispose();
    }

    private void OnSelectedChanged(TListingItemViewModel? item)
    {
        if (item == null)
        {
            Details = null;
            return;
        }

        var model = _store.Get(item.Id);

        if (model == null)
        {
            Details = null;
            return;
        }

        Details = CreateDetailsViewModel(model);
    }

    protected abstract TDetailsViewModel CreateDetailsViewModel(TModel model);

    protected abstract TListingItemViewModel CreateListingItemViewModel(TModel model);

    protected virtual void DisposeDetailsViewModel(TDetailsViewModel? detailsViewModel)
    {
        if (detailsViewModel is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void OnModelAdded(TModel model)
    {
        Listing.AddItem(CreateListingItemViewModel(model));
    }

    private void OnModelsLoaded(IEnumerable<TModel> collection)
    {
        Listing.Reset(collection.Select(CreateListingItemViewModel));
    }

    private void OnModelRemoved(TModel model)
    {
        TListingItemViewModel? removedItem = Listing.RemoveItem(model.Id);

        if (removedItem == null)
        {
            return;
        }

        if (Listing.SelectedItem?.Id == removedItem.Id)
        {
            Listing.SelectedItem = null;
            ResetDetails(null);
        }
    }

    private void OnModelUpdated(TModel model)
    {
        // TODO: This code this way or another already exacutes in scope of ListingViewModel
        // not sure if we have to do that twice -- aa
        var existingId = Listing.Items.FirstOrDefault(i => i.Id == model.Id)?.Id;

        if (existingId == null)
        {
            return;
        }

        Listing.ReplaceItem(CreateListingItemViewModel(model));

        if (Listing.SelectedItem?.Id == existingId)
        {
            ResetDetails(CreateDetailsViewModel(model));
        }
    }

    private void ResetDetails(TDetailsViewModel? detailsViewModel)
    {
        TDetailsViewModel? oldValue = Details;
        Details = detailsViewModel;
        DisposeDetailsViewModel(oldValue);
    }
}