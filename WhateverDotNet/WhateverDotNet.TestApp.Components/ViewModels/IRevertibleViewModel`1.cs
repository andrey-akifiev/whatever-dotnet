using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public interface IRevertibleViewModel<TModel>
    where TModel : class, ICloneable
{
    public bool IsDirty { get; }

    public ICommand RevertCommand { get; }

    public TModel CloneModel(TModel source);

    public void RaiseDirty();

    public void Revert();
}