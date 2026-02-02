using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class CollectionDirtyTrackingViewModel<TItemViewModel, TItemModel>
    : BaseViewModel, IDisposable
        where TItemViewModel : DirtyTrackingViewModel<TItemModel>
        where TItemModel : class
{
    private List<TItemModel> _originalModels = new();

    protected CollectionDirtyTrackingViewModel(ObservableCollection<TItemViewModel> itemViewModels)
    {
        Current = itemViewModels;
        Current.CollectionChanged += OnCollectionChanged;
        SubscribeItemsPropertyChanged(Current);

        Snapshot();

        AddCommand = new RelayCommand(
            execute: Add,
            canExecute: CanExecuteAddItemCommand);
        RevertCommand = new RelayCommand(
            execute: Revert,
            canExecute: CanExecuteRevertCommand);
    }

    public bool IsDirty =>
        _originalModels.Count() != Current.Count()
        || Current.Any(i => i.IsDirty);

    public ObservableCollection<TItemViewModel> Current { get; }

    public ICommand AddCommand { get; }

    public ICommand RevertCommand { get; }

    public virtual void Add()
        => Add(CreateNewItem());

    public virtual void Add(TItemModel itemModel)
        => Add(CreateNewItem(itemModel));

    public virtual void Add(TItemViewModel itemViewModel)
    {
        Current.Add(itemViewModel);
        RaiseDirty();
    }

    public virtual void AddRange(IEnumerable<TItemViewModel> itemViewModels)
    {
        if (itemViewModels == null)
        {
            return;
        }

        foreach (var itemViewModel in itemViewModels)
        {
            Current.Add(itemViewModel);
        }

        RaiseDirty();
    }

    public void Clone(TItemViewModel item)
    {
        TItemModel clonedModel = CloneModel(item.Current);
        TItemViewModel clonedItem = CreateNewItem(clonedModel);

        int index = Current.IndexOf(item);
        Current.Insert(index + 1, clonedItem);

        RaiseDirty();
    }

    public override void Dispose()
    {
        Current.CollectionChanged -= OnCollectionChanged;

        UnsubscribeItemsPropertyChanged(Current);

        base.Dispose();
    }

    public void Remove(TItemViewModel item)
    {
        Current.Remove(item);
        RaiseDirty();
    }

    public virtual void Revert()
    {
        Current.Clear();

        foreach (var item in _originalModels.Select(CreateNewItem))
        {
            Current.Add(item);
        }

        RaisePropertiesChanged(nameof(Current));
    }

    public virtual void ResetItems(IEnumerable<TItemViewModel> itemViewModels)
    {
        Current.Clear();

        if (itemViewModels == null)
        {
            return;
        }

        foreach (var itemViewModel in itemViewModels)
        {
            Current.Add(itemViewModel);
        }

        Snapshot();
        RaisePropertiesChanged(nameof(Current));
    }

    public void Snapshot()
    {
        _originalModels = Current
            .Select(i => CloneModel(i.Current))
            .ToList();
        RaiseDirty();
    }

    protected virtual bool CanExecuteAddItemCommand()
        => true;

    protected virtual bool CanExecuteRevertCommand()
        => IsDirty;

    protected abstract TItemModel CloneModel(TItemModel model);

    protected abstract TItemViewModel CreateNewItem();

    protected abstract TItemViewModel CreateNewItem(TItemModel model);

    protected void RaiseDirty()
    {
        OnPropertyChanged(nameof(IsDirty));
        // TODO
        // (RevertCommand as RelayCommand)?.NotifyCanExecuteChanged();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SubscribeItemsPropertyChanged(e.NewItems);
        UnsubscribeItemsPropertyChanged(e.OldItems);
        RaiseDirty();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DirtyTrackingViewModel<TItemViewModel>))
        {
            RaiseDirty();
        }
    }

    private void SubscribeItemsPropertyChanged(System.Collections.IList? collectionItems)
    {
        if (collectionItems == null)
        {
            return;
        }

        foreach (object? collectionItem in collectionItems)
        {
            SubscribeItemPropertyChanged(collectionItem);
        }
    }

    private void SubscribeItemPropertyChanged(object? collectionItem)
        => (collectionItem as TItemViewModel)
            ?.PropertyChanged += OnItemPropertyChanged;

    private void UnsubscribeItemsPropertyChanged(System.Collections.IList? collectionItems)
    {
        if (collectionItems == null)
        {
            return;
        }

        foreach (object? collectionItem in collectionItems)
        {
            UnsubscribeItemPropertyChanged(collectionItem);
        }
    }

    private void UnsubscribeItemPropertyChanged(object? collectionItem)
        => (collectionItem as TItemViewModel)
            ?.PropertyChanged -= OnItemPropertyChanged;
}
