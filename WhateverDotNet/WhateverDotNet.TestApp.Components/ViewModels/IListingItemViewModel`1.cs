namespace WhateverDotNet.TestApp.Components.ViewModels;

public interface IListingItemViewModel<TModel>
{
    Guid Id { get; }

    TModel Model { get; }
}