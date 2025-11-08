using Microsoft.Playwright;

namespace WhateverDotNet.Components;

public interface IApplicationPane
{
    public Task AssertIsDisabledAsync(string componentName);
    public Task AssertIsEnabledAsync(string componentName);
    public Task AssertIsNotVisibleAsync();
    public Task AssertIsNotVisibleAsync(string componentName);
    public Task AssertIsVisibleAsync();
    public Task AsserHasNoValidationAsync(string componentName);
    public Task AssertHasOptionAsync(string componentName, string optionName);
    public Task AssertHasTextAsync(string componentName, string expectedText);
    public Task AssertHasValidationAsync(string componentName, string? expectedValidationMessage = null);
    public Task ClickAsync(string componentName, LocatorClickOptions? options = null);
    public Task SelectItemAsync(string componentName, string itemText, LocatorClickOptions? options = null);
    public Task TypeAsync(string componentName, string text, ForceOptions? options = null);
    public Task WaitUntilLoadedAsync();
}