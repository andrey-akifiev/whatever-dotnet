namespace WhateverDotNet.TestApp.Components.Stores;

public interface IModelStore<TModel>
    where TModel : class, IModel
{
    public IReadOnlyList<TModel> Items { get; }

    TModel? Get(Guid id);

    void Add(TModel entity);

    void Remove(Guid id);

    void Update(TModel entity);

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(CancellationToken cancellationToken = default);
}