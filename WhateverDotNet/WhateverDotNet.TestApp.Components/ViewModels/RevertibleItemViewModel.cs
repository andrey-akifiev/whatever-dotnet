using CommunityToolkit.Mvvm.Input;

using System.ComponentModel;
using System.Windows.Input;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public class RevertibleItemViewModel<TModel>
    : RevertibleViewModel<TModel>,
        IDisposable,
        INotifyPropertyChanged,
        IRevertibleViewModel<TModel>
        where TModel : class, ICloneable, IEquatable<TModel>
{
    public event EventHandler<RevertibleItemViewModelActionRequestedEventArgs<RevertibleItemViewModel<TModel>>>? CloneRequested;

    public event EventHandler<RevertibleItemViewModelActionRequestedEventArgs<RevertibleItemViewModel<TModel>>>? RemoveRequested;

    protected RevertibleItemViewModel(TModel originalValue)
        : base(originalValue)
    {
        CloneCommand = new RelayCommand(OnCloneRequested);
        RemoveCommand = new RelayCommand(OnRemoveRequested);
    }

    public ICommand CloneCommand { get; }

    public ICommand RemoveCommand { get; }

    protected virtual void OnCloneRequested()
        => CloneRequested?.Invoke(
            this,
            new RevertibleItemViewModelActionRequestedEventArgs<RevertibleItemViewModel<TModel>>(this));

    protected virtual void OnRemoveRequested()
        => RemoveRequested?.Invoke(
            this,
            new RevertibleItemViewModelActionRequestedEventArgs<RevertibleItemViewModel<TModel>>(this));
}