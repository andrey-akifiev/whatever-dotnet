using Microsoft.Playwright;

using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;

namespace WhateverDotNet.Components.Selects;

public class DropdownComponent : BaseSelectComponent, ISelectComponent
{
    public DropdownComponent(IScenarioContext scenarioContext, SelectComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    protected override string GetXPathItem(string? item = null)
    {
        string? xPathItem = GetOptions().XPathItem;

        if (xPathItem == null)
        {
            throw new OptionNotDefinedException(nameof(_options.XPathItem), this);
        }
        
        if (item == null)
        {
            return xPathItem;
        }
        
        return $"{xPathItem}[normalize-space()=\"{item}\"]";
    }

    public override Task ClickAsync(LocatorClickOptions? options = null)
    {
        return GetLocator(GetXPathComponent()).ClickAsync();
    }
    
    public override async Task<string> TypeAsync(string text, ForceOptions? options = null)
    {
        ILocator inputLocator = GetLocator($"{GetXPathComponent()}{GetOptions().XPathInput}");

        await inputLocator
            .ClickAsync(new LocatorClickOptions { Force = true })
            .ConfigureAwait(false);
        await inputLocator
            .ClearAsync()
            .ConfigureAwait(false);
        await inputLocator
            .FillAsync(text)
            .ConfigureAwait(false);

        return text;
    }
}