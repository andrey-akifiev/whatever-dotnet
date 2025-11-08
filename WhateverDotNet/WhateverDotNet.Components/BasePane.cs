using Microsoft.Playwright;
using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;
using WhateverDotNet.Components.Exceptions;

namespace WhateverDotNet.Components;

public class PaneOptions
{
    public string? BrowserTabTitle { get; set; }
    public string? Caption { get; set; }
    public IComponent[]? Components { get; set; }
    
    public string? Description { get; set; }
    public bool? IsTemplate { get; set; }
    
    public string? OnLoadRequestAlias { get; set; }
    public string? OnLoadRequestEndpoint { get; set; }
    
    public string? OnSubmitRequestAlias { get; set; }
    public string? OnSubmitRequestEndpoint { get; set; }
    public string? OnSubmitVariableName { get; set; }
    
    public string? XPathCaption { get; set; }
    public string? XPathComponent { get; set; }
    public string? XPathLoader { get; set; }
    public string? XPathParent { get; set; }
}

public class BasePane : IApplicationPane
{
    protected readonly PaneOptions _options;
    protected readonly IScenarioContext _scenarioContext;
    
    private readonly Dictionary<string, IComponent> _componentRegistry = new Dictionary<string, IComponent>();
    
    public BasePane(IScenarioContext scenarioContext, PaneOptions options)
    {
        _scenarioContext = scenarioContext;
        _options = options;
    }
    
    public virtual Task AssertIsDisabledAsync(string componentName)
    {
        return GetComponent(componentName).AssertIsDisabledAsync();
    }

    public virtual Task AssertIsEnabledAsync(string componentName)
    {
        return GetComponent(componentName).AssertIsEnabledAsync();
    }
    
    public virtual Task AssertIsNotVisibleAsync()
    {
        return Assertions.Expect(GetLocator(GetXPathComponent())).ToBeHiddenAsync();
    }
    
    public virtual Task AssertIsNotVisibleAsync(string componentName)
    {
        return GetComponent(componentName).AssertIsNotVisibleAsync();
    }

    public virtual Task AssertIsVisibleAsync()
    {
        return Assertions.Expect(GetComponent()).ToBeVisibleAsync();
    }

    public Task AsserHasNoValidationAsync(string componentName)
    {
        return GetComponent(componentName).AssertHasNoValidationAsync();
    }

    public virtual Task AssertHasNoValidationAsync(string componentName)
    {
        return GetComponent(componentName).AssertHasNoValidationAsync();
    }
    
    public virtual Task AssertHasOptionAsync(string componentName, string optionName)
    {
        return GetComponent(componentName).AssertHasOptionAsync(optionName);
    }
    
    public virtual Task AssertHasTextAsync(string componentName, string expectedText)
    {
        return GetComponent(componentName).AssertHasTextAsync(expectedText);
    }
    
    public virtual Task AssertHasValidationAsync(string componentName, string? expectedValidationMessage = null)
    {
        return GetComponent(componentName).AssertHasValidationAsync(expectedValidationMessage);
    }
    
    public virtual Task ClickAsync(string componentName, LocatorClickOptions? options = null)
    {
        return GetComponent(componentName).ClickAsync(options);
    }

    public virtual async Task ClickCaptionAsync()
    {
        if (GetOptions().XPathCaption == null)
        {
            throw new OptionNotDefinedException(nameof(_options.XPathCaption), this);
        }
        
        await GetLocator($"{GetXPathParent()}{GetOptions().XPathCaption}")
            .ClickAsync(new LocatorClickOptions { Force = true })
            .ConfigureAwait(false);
    }
    
    public virtual PaneOptions GetOptions()
    {
        return _options;
    }
    
    public virtual Task SelectItemAsync(string componentName, string itemName, LocatorClickOptions? options = null)
    {
        throw new NotImplementedException();
    }
    
    public virtual Task TypeAsync(string componentName, string text, ForceOptions? options = null)
    {
        return GetComponent(componentName).TypeAsync(text, options);
    }
    
    public virtual async Task WaitUntilLoadedAsync()
    {
        string? xPathLoader = GetOptions().XPathLoader;

        if (string.IsNullOrWhiteSpace(xPathLoader))
        {
            return;
        }

        await GetLocator(xPathLoader)
            .WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Detached
            })
            .ConfigureAwait(false);
    }

    public ILocator GetComponent()
    {
        return GetLocator(GetXPathComponent());
    }
    
    public IComponent GetComponent(string componentName)
    {
        // string normalizedComponentName = NormalizeName(componentName);
        //
        // if (_componentRegistry.TryGetValue(normalizedComponentName, out var component))
        // {
        //     return component;
        // }
        //
        // ComponentOptions[] predefinedComponents = GetPredefinedComponentsOrThrow();
        //
        // // TODO: Complex components
        //
        // var predefinedComponent = predefinedComponents.FirstOrDefault(cmp => cmp.Name == normalizedComponentName);

        throw new NotImplementedException();
    }

    protected ILocator GetLocator(string selector)
    {
        return GetPage().Locator(selector);
    }

    protected IPage GetPage()
    {
        return _scenarioContext.GetBrowserPage();
    }
    
    protected string GetXPathComponent()
    {
        string? xPathComponent = GetOptions().XPathComponent;

        if (xPathComponent == null)
        {
            throw new OptionNotDefinedException(nameof(_options.XPathComponent), this);
        }

        return $"{GetXPathParent()}{xPathComponent}";
    }

    protected string GetXPathParent()
    {
        return GetOptions()?.XPathParent ?? string.Empty;
    }
    //
    // protected bool IsComplexComponent(ComponentOptions componentOptions)
    // {
    //     
    // }
    //
    // private ComponentOptions[] GetPredefinedComponentsOrThrow()
    // {
    //     ComponentOptions[]? predefinedComponents = GetOptions().ComponentOptions;
    //     
    //     if (predefinedComponents == null || predefinedComponents.Length == 0)
    //     {
    //         throw new ComponentNotSpecifiedException();
    //     }
    //
    //     return predefinedComponents;
    // }
    //
    private string NormalizeName(string name)
    {
        return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(name);
    }
}