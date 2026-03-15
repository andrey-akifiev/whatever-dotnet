using System.ComponentModel;

using WhateverDotNet.TestApp.Components.Stores;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public class BaseItemViewModel<TModel>
    : BaseViewModel,
        IDisposable,
        IItemViewModel<TModel>,
        INotifyPropertyChanged
        where TModel : class, ICloneable, IModel
{
    public BaseItemViewModel(TModel model)
    {
        Model = CloneModel(model);
    }

    public Guid Id => Model.Id;

    public TModel Model { get; private set; }

    public TModel CloneModel(TModel source)
        => (TModel)source.Clone();
}