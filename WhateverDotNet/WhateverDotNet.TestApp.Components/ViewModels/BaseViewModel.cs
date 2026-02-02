using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WhateverDotNet.TestApp.Components.ViewModels;

public class BaseViewModel : IDisposable, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public virtual void Dispose()
    {
    }

    protected virtual void OnPropertyChanged([CallerMemberName]string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void RaisePropertiesChanged(params string[] propertyNames)
    {
        foreach (var property in propertyNames)
        {
            OnPropertyChanged(property);
        }
    }
}