using System.Collections.ObjectModel;

using WhateverDotNet.TestApp.Components.Controls;

namespace WhateverDotNet.TestApp.Components.Stores;

public abstract class BaseModelStore<TModel>
    : IEnlistableItemsStore<TModel>,
      IModelStore<TModel>,
      IModelStoreEvents<TModel>
        where TModel : class, ICloneable, IEnlistableModel, IModel
{
    public event Action<TModel>? Added;
    public event Action<IEnumerable<TModel>>? Loaded;
    public event Action? SelectedItemChanged;
    public event Action<TModel>? Updated;
    public event Action<TModel>? Removed;

    protected BaseModelStore()
    {
        Items = new ObservableCollection<TModel>();
    }

    IReadOnlyList<TModel> IModelStore<TModel>.Items => Items;

    public ObservableCollection<TModel> Items { get; }

    public TModel? SelectedItem { get; private set; }

    public virtual void Add(TModel entity)
    {
        Items.Add(entity);
        OnAdded(entity);
    }

    public TModel? Get(Guid id)
    {
        return Items.FirstOrDefault(x => x.Id == id);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Items.Clear();

        IEnumerable<TModel> items = await LoadAsyncImpl(cancellationToken);

        foreach (var item in items)
        {
            Items.Add(item);
        }

        OnLoaded(items);
    }

    public virtual void Remove(TModel model)
    {
        Items.Remove(model);
        OnRemoved(model);
    }

    public virtual void Remove(Guid id)
    {
        TModel? entity = Get(id);

        if (entity == null)
        {
            return;
        }

        Remove(entity);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await SaveAsyncImpl(Items, cancellationToken);
    }

    public void SelectItem(TModel? item)
    {
        SelectedItem = item;
        SelectedItemChanged?.Invoke();
    }

    public void SelectItem(Guid id)
    {
        SelectedItem = Get(id);
        SelectedItemChanged?.Invoke();
    }

    public virtual void Update(TModel entity)
    {
        TModel? existingItem = Items.FirstOrDefault(x => x.Id == entity.Id);

        if (existingItem is null)
        {
            throw new InvalidOperationException("Entity not found");
        }

        int index = Items.IndexOf(existingItem);
        Items.RemoveAt(index);
        Items.Insert(index, entity);

        OnUpdated(entity);
    }

    protected abstract Task<IEnumerable<TModel>> LoadAsyncImpl(CancellationToken cancellationToken);

    protected abstract Task SaveAsyncImpl(IReadOnlyCollection<TModel> items, CancellationToken cancellationToken);

    protected virtual void OnAdded(TModel entity)
    {
        Added?.Invoke(entity);
    }

    protected virtual void OnLoaded(IEnumerable<TModel> items)
    {
        Loaded?.Invoke(items);
    }

    protected virtual void OnUpdated(TModel entity)
    {
        Updated?.Invoke(entity);
    }

    protected virtual void OnRemoved(TModel entity)
    {
        Removed?.Invoke(entity);
    }
}