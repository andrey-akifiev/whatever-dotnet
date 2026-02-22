using CommunityToolkit.Mvvm.Input;

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

using WhateverDotNet.TestApp.Components.Models;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class RevertibleCollectionViewModel<TItemViewModel, TItemModel>
    : BaseViewModel,
        ICollection<TItemViewModel>,
        IDisposable,
        IEnumerable,
        IEnumerable<TItemViewModel>,
        IList<TItemViewModel>,
        IReadOnlyCollection<TItemViewModel>,
        IReadOnlyList<TItemViewModel>,
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        IRevertibleViewModel<TItemModel>
        where TItemViewModel
            : RevertibleItemViewModel<TItemModel>, IRevertibleViewModel<TItemModel>
        where TItemModel
            : class, ICloneable, IEquatable<TItemModel>, ISampleProvider<TItemModel>
{
    private bool _isDirtyPrev = false;
    private List<TItemModel> _originalModels = new();

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected RevertibleCollectionViewModel(ObservableCollection<TItemViewModel> itemViewModels)
    {
        Current = itemViewModels;

        Current.CollectionChanged += OnItemsChanged;

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

    public int Count => Current.Count;

    public bool IsReadOnly => false;

    public TItemViewModel this[int index]
    {
        get => Current[index];
        set => Current[index] = value;
    }

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

    public void Clear()
        => Current.Clear();

    public void CloneItem(TItemViewModel item)
    {
        TItemModel clonedModel = CloneModel(item.Current);
        TItemViewModel clonedItem = CreateNewItem(clonedModel);

        int index = Current.IndexOf(item);
        Current.Insert(index + 1, clonedItem);

        RaiseDirty();
    }

    public TItemModel CloneModel(TItemModel model)
        => (TItemModel)model.Clone();

    public bool Contains(TItemViewModel item)
        => Current.Contains(item);

    public void CopyTo(TItemViewModel[] array, int arrayIndex)
        => Current.CopyTo(array, arrayIndex);

    public override void Dispose()
    {
        Current.CollectionChanged -= OnItemsChanged;

        UnsubscribeItemsEvents(Current);

        base.Dispose();
    }

    public int IndexOf(TItemViewModel item)
        => Current.IndexOf(item);

    public void Insert(int index, TItemViewModel item)
        => Current.Insert(index, item);

    public void RaiseDirty()
    {
        bool isDirtyCur = IsDirty;

        if (_isDirtyPrev == isDirtyCur)
        {
            return;
        }

        OnPropertyChanged(nameof(IsDirty));
        RaiseRevertCanExecute();
    }

    public bool Remove(TItemViewModel item)
    {
        bool innerRemoveResult = Current.Remove(item);
        if (innerRemoveResult)
        {
            RaiseDirty();
        }

        return innerRemoveResult;
    }

    public void RemoveAt(int index)
    {
        Current?.RemoveAt(index);
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

    protected virtual TItemViewModel CreateNewItem()
        => CreateNewItem(TItemModel.CreateSample());

    protected abstract TItemViewModel CreateNewItem(TItemModel model);

    protected virtual void RaiseRevertCanExecute()
        => (RevertCommand as RelayCommand)?.NotifyCanExecuteChanged();

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SubscribeItemsEvents(e.NewItems);
        UnsubscribeItemsEvents(e.OldItems);
        RaiseDirty();

        CollectionChanged?.Invoke(sender, e);
    }

    private void OnItemCloneRequested(
        object? sender,
        RevertibleItemViewModelActionRequestedEventArgs<RevertibleItemViewModel<TItemModel>> e)
    {
        CloneItem((TItemViewModel)e.Item);
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IRevertibleViewModel<>.IsDirty))
        {
            RaiseDirty();
        }
    }

    private void OnItemRemoveRequested(
        object? sender,
        RevertibleItemViewModelActionRequestedEventArgs<RevertibleItemViewModel<TItemModel>> e)
    {
        Remove((TItemViewModel)e.Item);
    }

    private void SubscribeItemsEvents(IList? collectionItems)
    {
        if (collectionItems == null)
        {
            return;
        }

        foreach (object? collectionItem in collectionItems)
        {
            SubscribeItemEvents(collectionItem);
        }
    }

    private void SubscribeItemEvents(object? collectionItem)
    {
        TItemViewModel? item = collectionItem as TItemViewModel;

        if (item == null)
        {
            return;
        }

        item.CloneRequested += OnItemCloneRequested;
        item.PropertyChanged += OnItemPropertyChanged;
        item.RemoveRequested += OnItemRemoveRequested;
    }

    private void UnsubscribeItemsEvents(IList? collectionItems)
    {
        if (collectionItems == null)
        {
            return;
        }

        foreach (object? collectionItem in collectionItems)
        {
            UnsubscribeItemEvents(collectionItem);
        }
    }

    private void UnsubscribeItemEvents(object? collectionItem)
    {
        TItemViewModel? item = collectionItem as TItemViewModel;

        if (item == null)
        {
            return;
        }

        item.CloneRequested -= OnItemCloneRequested;
        item.PropertyChanged -= OnItemPropertyChanged;
        item.RemoveRequested -= OnItemRemoveRequested;
    }

    public IEnumerator<TItemViewModel> GetEnumerator()
        => Current.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}