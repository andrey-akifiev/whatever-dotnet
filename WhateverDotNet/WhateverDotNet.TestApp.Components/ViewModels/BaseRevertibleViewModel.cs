using CommunityToolkit.Mvvm.Input;

using System.ComponentModel;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class BaseRevertibleViewModel<TModel>
    : BaseViewModel,
        IDisposable,
        INotifyPropertyChanged,
        IRevertibleViewModel<TModel>
        where TModel : class, ICloneable
{
    protected BaseRevertibleViewModel(TModel originalValue)
    {
        Current = originalValue;

        Snapshot();

        RevertCommand = new RelayCommand(
            execute: Revert,
            canExecute: CanExecuteRevertCommand);
    }

    public bool IsDirty => !AreEqual(Current, Original);

    public TModel Current { get; private set; }

    public TModel? Original { get; private set; }

    public ICommand RevertCommand { get; }

    public abstract TModel CloneModel(TModel source);

    public virtual void RaiseDirty()
        => OnPropertyChanged(nameof(IsDirty));

    public void Revert()
    {
        Current = CloneModel(Original);
        RaiseDirty();
    }

    protected abstract bool AreEqual(TModel? a, TModel? b);

    protected virtual bool CanExecuteRevertCommand() => IsDirty;

    protected virtual void RaiseRevertCanExecute()
        => (RevertCommand as RelayCommand)?.NotifyCanExecuteChanged();

    protected void Snapshot()
    {
        Original = CloneModel(Current);
        RaiseDirty();
    }
}