using Microsoft.Playwright;

using WhateverDotNet.Components.Panes;

namespace WhateverDotNet.Components;

public interface IApplicationPane : IApplicationPane<PaneOptions>
{
}

public interface IApplicationPane<TPaneOptions>
{
    public Task AssertIsDisabledAsync(string componentName);
    public Task AssertIsEnabledAsync(string componentName);
    public Task AssertIsNotVisibleAsync();
    public Task AssertIsNotVisibleAsync(string componentName);
    public Task AssertIsVisibleAsync();
    public Task AssertHasNoValidationAsync(string componentName);
    public Task AssertHasOptionAsync(string componentName, string optionName);
    public Task AssertHasTextAsync(string componentName, string expectedText);
    public Task AssertHasValidationAsync(string componentName, string? expectedValidationMessage = null);
    public Task ClickAsync(string componentName, LocatorClickOptions? options = null);
    public Task ClickCaptionAsync();
    public object? GetComponent(string componentName);
    public TPaneOptions GetOptions();
    public Task<string> SelectItemAsync(string componentName, string itemText, LocatorClickOptions? options = null);
    public Task TypeAsync(string componentName, string text, ForceOptions? options = null);
    public Task WaitUntilLoadedAsync();
}