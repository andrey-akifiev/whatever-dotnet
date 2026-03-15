using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public interface IViewModel<TModel>
    where TModel : class, IModel
{
    public Guid Id { get; }

    public TModel Model { get; }
}