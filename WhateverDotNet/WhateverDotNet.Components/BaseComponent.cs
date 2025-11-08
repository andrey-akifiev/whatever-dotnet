using Microsoft.Playwright;

using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;

namespace WhateverDotNet.Components;

public class BaseComponent<TOptions> : IComponent
    where TOptions : ComponentOptions
{
    protected readonly IScenarioContext _scenarioContext;
    protected readonly TOptions _options;

    public BaseComponent(IScenarioContext scenarioContext, TOptions options)
    {
        _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
        _options = options;
    }

    public virtual Task AssertIsDisabledAsync()
    {
        return Assertions
            .Expect(GetComponent())
            .ToBeDisabledAsync();
    }

    public virtual Task AssertIsEnabledAsync()
    {
        return Assertions
            .Expect(GetComponent())
            .ToBeEnabledAsync();
    }

    public virtual Task AssertIsNotVisibleAsync()
    {
        return Assertions
            .Expect(GetComponent())
            .ToBeHiddenAsync();
    }

    public virtual Task AssertHasNoValidationAsync()
    {
        return Assertions
            .Expect(GetLocator(GetXPathValidation()))
            .ToBeHiddenAsync();
    }

    public virtual Task AssertHasOptionAsync(string optionName)
    {
        return Assertions
            .Expect(GetLocator(GetXPathItem(optionName)))
            .ToBeVisibleAsync();
    }

    public virtual Task AssertHasTextAsync(string expectedText)
    {
        return Assertions
            .Expect(GetComponent())
            .ToHaveTextAsync(expectedText);
    }

    public virtual Task AssertHasValidationAsync(string? expectedValidationMessage = null)
    {
        if (string.IsNullOrEmpty(expectedValidationMessage))
        {
            return Assertions
                .Expect(GetLocator(GetXPathValidation()))
                .ToBeVisibleAsync();
        }
        
        return Assertions
            .Expect(GetLocator(GetXPathValidation()))
            .ToHaveTextAsync(expectedValidationMessage);
    }

    public virtual Task ClickAsync(LocatorClickOptions? options = null)
    {
        return GetComponent().ClickAsync(options);
    }

    public ILocator GetComponent()
    {
        return GetLocator(GetXPathComponent());
    }

    public ILocator GetLocator(string selector)
    {
        return GetPage().Locator(selector);
    }

    public virtual ComponentOptions GetOptions()
    {
        return _options;
    }

    public virtual Task<string?> GetTextAsync()
    {
        return GetComponent().TextContentAsync();
    }

    public virtual Task<string> TypeAsync(string text, ForceOptions? options = null)
    {
        throw new NotSupportedException($"'{nameof(TypeAsync)}' function is not supported by '{GetType().Name}' component.");
    }

    protected IPage GetPage()
    {
        return _scenarioContext.GetBrowserPage();
    }
    
    protected virtual string GetXPathComponent()
    {
        var options = GetOptions();

        if (!string.IsNullOrWhiteSpace(options?.XPathComponent))
        {
            return $"{GetXPathParent()}{options.XPathComponent}";
        }
        
        throw new NotImplementedException();
    }

    protected virtual string GetXPathItem(string? item = null)
    {
        if (string.IsNullOrWhiteSpace(GetOptions().XPathItem))
        {
            throw new OptionNotDefinedException(nameof(_options.XPathItem), this);
        }

        if (item == null)
        {
            return $"{GetXPathComponent()}{GetOptions().XPathItem}";
        }
        
        return $"{GetXPathComponent()}{GetOptions().XPathItem}[normalize-space()=\"{item}\"]";
    }
    
    protected string GetXPathParent()
    {
        return GetOptions().XPathParent ?? string.Empty;
    }
    
    protected string GetXPathValidation()
    {
        var xPathValidation = GetOptions().XPathValidation;
        
        if (string.IsNullOrWhiteSpace(xPathValidation))
        {
            throw new OptionNotDefinedException(nameof(_options.XPathValidation), this);
        }

        return $"{GetXPathComponent()}{xPathValidation}";
    }
}