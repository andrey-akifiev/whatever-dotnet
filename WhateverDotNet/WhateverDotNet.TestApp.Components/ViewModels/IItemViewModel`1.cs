using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public interface IItemViewModel<TModel>
    : IViewModel<TModel>
        where TModel : class, ICloneable, IModel
{
    public TModel CloneModel(TModel source);
}