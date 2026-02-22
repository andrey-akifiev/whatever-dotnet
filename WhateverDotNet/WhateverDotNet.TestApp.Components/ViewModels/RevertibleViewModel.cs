using CommunityToolkit.Mvvm.Input;

using System.ComponentModel;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public class RevertibleViewModel<TModel>
    : BaseViewModel,
        IDisposable,
        INotifyPropertyChanged,
        IRevertibleViewModel<TModel>
        where TModel : class, ICloneable, IEquatable<TModel>
{
    private bool _isDirtyPrev = false;

    protected RevertibleViewModel(TModel originalValue)
    {
        Current = CloneModel(originalValue);

        Snapshot();

        RevertCommand = new RelayCommand(
            execute: Revert,
            canExecute: CanExecuteRevertCommand);
    }

    public bool IsDirty => !AreEqual(Current, Original);

    public TModel Current { get; private set; }

    public TModel? Original { get; private set; }

    public ICommand RevertCommand { get; }

    public TModel CloneModel(TModel source)
        => (TModel)source.Clone();

    public virtual void RaiseDirty()
    {
        bool isDirtryCur = IsDirty;

        if (_isDirtyPrev == isDirtryCur)
        {
            return;
        }

        _isDirtyPrev = isDirtryCur;

        OnPropertyChanged(nameof(IsDirty));
        RaiseRevertCanExecute();
    }

    public virtual void Revert()
    {
        if (Original != null)
        {
            Current = CloneModel(Original);
            RaisePropertiesChanged(nameof(Current));
        }
    }

    protected virtual bool AreEqual(TModel? a, TModel? b)
        => a?.Equals(b) ?? false;

    protected virtual bool CanExecuteRevertCommand() => IsDirty;

    protected override void RaisePropertiesChanged(params string[] propertyNames)
    {
        base.RaisePropertiesChanged(propertyNames);
        RaiseDirty();
    }

    protected virtual void RaiseRevertCanExecute()
        => (RevertCommand as RelayCommand)?.NotifyCanExecuteChanged();

    protected void Snapshot()
    {
        Original = CloneModel(Current);
        RaiseDirty();
    }
}