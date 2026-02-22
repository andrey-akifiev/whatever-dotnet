namespace WhateverDotNet.TestApp.Components.ViewModels;

public class RevertibleItemViewModelActionRequestedEventArgs<TItemViewModel> : EventArgs
{
    public RevertibleItemViewModelActionRequestedEventArgs(TItemViewModel item)
    {
        Item = item;
    }

    public TItemViewModel Item { get; }
}