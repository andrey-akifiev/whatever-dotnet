namespace WhateverDotNet.TestApp.Components.Stores;

public abstract class BaseModelStore<TModel>
    : IModelStore<TModel>, IModelStoreEvents<TModel>
        where TModel : class, IModel
{
    protected readonly List<TModel> _items = new();

    public event Action<TModel>? Added;
    public event Action<IEnumerable<TModel>>? Loaded;
    public event Action<TModel>? Updated;
    public event Action<TModel>? Removed;

    public IReadOnlyList<TModel> Items => _items;

    public virtual void Add(TModel entity)
    {
        if (_items.Any(i => i.Id  == entity.Id))
        {
            throw new InvalidOperationException("Entity already exists");
        }

        _items.Add(entity);
        OnAdded(entity);
    }

    public TModel? Get(Guid id)
    {
        return _items.FirstOrDefault(x => x.Id == id);
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        _items.Clear();

        IEnumerable<TModel> items = await LoadAsyncImpl(cancellationToken);

        _items.AddRange(items);

        OnLoaded(items);
    }

    public virtual void Remove(Guid id)
    {
        TModel? entity = Get(id);

        if (entity == null)
        {
            return;
        }

        _items.Remove(entity);
        OnRemoved(entity);
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        await SaveAsyncImpl(_items, cancellationToken);
    }

    public virtual void Update(TModel entity)
    {
        var index = _items.FindIndex(x => x.Id == entity.Id);

        if (index < 0)
        {
            throw new InvalidOperationException("Entity not found");
        }

        _items[index] = entity;
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