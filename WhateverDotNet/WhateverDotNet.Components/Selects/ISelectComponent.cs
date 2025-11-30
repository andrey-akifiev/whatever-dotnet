namespace WhateverDotNet.Components.Selects;

public interface ISelectComponent
{
    Task<string> SelectItemAsync(string item, ForceOptions? options = null);
    
    Task<string> SelectItemsAsync();
}