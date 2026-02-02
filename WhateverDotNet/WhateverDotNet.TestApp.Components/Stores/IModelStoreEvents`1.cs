namespace WhateverDotNet.TestApp.Components.Stores;

public interface IModelStoreEvents<TModel>
{
    event Action<TModel>? Added;
    event Action<IEnumerable<TModel>> Loaded;
    event Action<TModel> Updated;
    event Action<TModel> Removed;
}