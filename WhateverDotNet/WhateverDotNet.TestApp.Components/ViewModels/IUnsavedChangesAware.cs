namespace WhateverDotNet.TestApp.Components.ViewModels;

public interface IUnsavedChangesAware
{
    bool HasUnsavedChanges { get; }
}

