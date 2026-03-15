using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public class BaseListingViewModel<TListingItemViewModel, TModel>
    : BaseViewModel
        where TListingItemViewModel: class, IItemViewModel<TModel>
        where TModel : class, IModel, ICloneable
{
    private TListingItemViewModel? _selectedItem;

    public event Action<TListingItemViewModel>? SelectedChanged;

    public BaseListingViewModel(IEnumerable<TListingItemViewModel> itemViewModels)
    {
        Items = new ObservableCollection<TListingItemViewModel>(itemViewModels);
    }

    public ObservableCollection<TListingItemViewModel> Items { get; }

    public void AddItem(TListingItemViewModel item)
    {
        Items.Add(item);
    }

    public void AddRange(IEnumerable<TListingItemViewModel> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public void ClearSelection()
    {
        SelectedItem = null;
    }

    public void ReplaceItem(TListingItemViewModel newItem)
    {
        TListingItemViewModel? existing = Items.FirstOrDefault(i => i.Id == newItem.Id);
        
        if (existing == null)
        {
            return;
        }

        int index = Items.IndexOf(existing);
        Items[index] = newItem;
    }

    public TListingItemViewModel? RemoveItem(Guid id)
    {
        TListingItemViewModel? item = Items.FirstOrDefault(i => i.Id == id);

        if (item != null)
        {
            Items.Remove(item);
        }

        return item;
    }

    public void Reset(IEnumerable<TListingItemViewModel> items)
    {
        Items.Clear();
        AddRange(items);
    }

    public TListingItemViewModel? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (!Equals(value, _selectedItem))
            {
                _selectedItem = value;
                SelectedChanged?.Invoke(value);
                OnPropertyChanged();
            }
        }
    }
}