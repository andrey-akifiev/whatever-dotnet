using Microsoft.Playwright;

namespace WhateverDotNet.Components;

public interface IComponent
{
    Task AssertIsDisabledAsync();
    Task AssertIsEnabledAsync();
    Task AssertIsNotVisibleAsync();
    Task AssertHasNoValidationAsync();
    Task AssertHasOptionAsync(string optionName);
    Task AssertHasTextAsync(string expectedText);
    Task AssertHasValidationAsync(string? expectedValidationMessage = null);
    Task ClickAsync(LocatorClickOptions? options = null);
    ComponentOptions GetOptions();
    Task<string?> GetTextAsync();
    Task<string> TypeAsync(string text, ForceOptions? options = null);
}