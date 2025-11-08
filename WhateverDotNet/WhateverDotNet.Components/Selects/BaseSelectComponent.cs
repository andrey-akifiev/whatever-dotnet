using Microsoft.Playwright;

using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;
using WhateverDotNet.Components.Buttons;

namespace WhateverDotNet.Components.Selects;

public abstract class BaseSelectComponent : BaseComponent<SelectComponentOptions>, ISelectComponent
{
    protected BaseSelectComponent(IScenarioContext scenarioContext, SelectComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    public override Task ClickAsync(LocatorClickOptions? options = null)
    {
        return GetLocator(GetXPathInput()).ClickAsync(options);
    }

    public async Task CollapseAsync()
    {
        if (await IsExpandedAsync().ConfigureAwait(false))
        {
            return;
        }
        
        await ClickAsync().ConfigureAwait(false);
        await GetPage()
            .WaitForTimeoutAsync(100)
            .ConfigureAwait(false);
    }

    public async Task ExpandAsync()
    {
        bool isExpanded = await IsExpandedAsync().ConfigureAwait(false);

        await GetPage()
            .WaitForTimeoutAsync(100)
            .ConfigureAwait(false);

        if (isExpanded)
        {
            return;
        }

        await ClickAsync().ConfigureAwait(false);
        await GetPage()
            .WaitForTimeoutAsync(100)
            .ConfigureAwait(false);
    }

    public override SelectComponentOptions GetOptions()
    {
        return _options;
    }

    public async Task<string> SelectItemAsync(string? item, ForceOptions? options = null)
    {
        if (GetOptions().Expandable != null && GetOptions().Expandable!.Value)
        {
            await ExpandAsync().ConfigureAwait(false);
        }
        
        var embeddedActions = GetOptions().EmbeddedActions;
        
        if (embeddedActions is { Length: > 0 } && item != null)
        {
            string normalizeName = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(item);
            
            ButtonComponentOptions? definedAction = embeddedActions.FirstOrDefault(x => x.Name == normalizeName);
            
            if (definedAction != null)
            {
                if (string.IsNullOrWhiteSpace(definedAction.XPathComponent))
                {
                    throw new OptionNotDefinedException(nameof(definedAction.XPathComponent));
                }

                if (definedAction.OnClickAsync == null)
                {
                    throw new OptionNotDefinedException(nameof(definedAction.OnClickAsync));
                }
                
                await GetLocator(definedAction.XPathComponent)
                    .ClickAsync()
                    .ConfigureAwait(false);
                await definedAction
                    .OnClickAsync()
                    .ConfigureAwait(false);
                return item;
            }
        }
        
        await GetLocator(GetXPathItem(item))
            .ClickAsync()
            .ConfigureAwait(false);
        await GetPage()
            .WaitForTimeoutAsync(100)
            .ConfigureAwait(false);
        
        return item!;
    }
    
    public override async Task<string> TypeAsync(string text, ForceOptions? options = null)
    {
        await GetLocator(GetXPathInput())
            .FillAsync(text)
            .ConfigureAwait(false);
        return text;
    }

    protected string GetXPathInput()
    {
        var xPathInput = GetOptions().XPathInput;

        if (string.IsNullOrWhiteSpace(xPathInput))
        {
            throw new OptionNotDefinedException(nameof(_options.XPathInput), this);
        }
        
        return $"{GetXPathComponent()}{xPathInput}";
    }
    
    protected Task<bool> IsExpandedAsync()
    {
        return GetLocator($"({GetXPathItem()})[1]").IsVisibleAsync();
    }
}