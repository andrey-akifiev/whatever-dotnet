using CommunityToolkit.Mvvm.Input;

using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public abstract class DirtyTrackingViewModel<TModel>
    : BaseViewModel
{
    protected DirtyTrackingViewModel(TModel model)
    {
        Current = CloneModel(model);
        Original = CloneModel(model);

        RevertCommand = new RelayCommand(Revert, () => IsDirty);
    }

    public TModel Current { get; private set; }
    public TModel Original { get; private set; }

    public virtual bool IsDirty => !AreEqual(Current, Original);

    public ICommand RevertCommand { get; }

    public virtual void Revert()
    {
        Current = CloneModel(Original);
        OnPropertyChanged(nameof(IsDirty));
    }

    public void RaiseDirty() => OnPropertyChanged(nameof(IsDirty));

    protected abstract bool AreEqual(TModel a, TModel b);

    protected abstract TModel CloneModel(TModel source);

    protected override void RaisePropertiesChanged(params string[] propertyNames)
    {
        base.RaisePropertiesChanged(propertyNames);
        OnPropertyChanged(nameof(IsDirty));
        RaiseRevertCanExecute();
    }

    protected void RaiseRevertCanExecute()
        => (RevertCommand as RelayCommand)?.NotifyCanExecuteChanged();
}