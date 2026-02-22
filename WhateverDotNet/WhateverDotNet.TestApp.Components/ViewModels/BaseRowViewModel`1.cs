using CommunityToolkit.Mvvm.Input;

using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class BaseRowViewModel<TModel>
    : RevertibleViewModel<TModel>
        where TModel : class, ICloneable, IEquatable<TModel>
{
    private readonly Action<BaseRowViewModel<TModel>> _onClone;
    private readonly Action<BaseRowViewModel<TModel>> _onDelete;

    public event EventHandler? CloneRequested;
    public event EventHandler? RemoveRequested;

    public BaseRowViewModel(
        TModel model,
        Action<BaseRowViewModel<TModel>> onClone,
        Action<BaseRowViewModel<TModel>> onDelete)
        : base(model)
    {
        _onClone = onClone;
        _onDelete = onDelete;

        CloneCommand = new RelayCommand(() => _onClone(this));
        DeleteCommand = new RelayCommand(() => _onDelete(this));
    }

    public ICommand CloneCommand { get; }

    public ICommand DeleteCommand { get; }

    public TModel GetData()
    {
        return CloneModel(Current);
    }

    protected virtual void OnCloneRequested()
        => CloneRequested?.Invoke(this, EventArgs.Empty);

    protected virtual void OnRemoveRequested()
        => RemoveRequested?.Invoke(this, EventArgs.Empty);
}