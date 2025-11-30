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

    public virtual async Task CollapseAsync()
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

    public virtual async Task ExpandAsync()
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

    public virtual async Task<string> SelectItemAsync(string? item, ForceOptions? options = null)
    {
        if (item != null && item.ToUpperInvariant() == nameof(SelectItems.ALL))
        {
            return await SelectItemsAsync().ConfigureAwait(false);
        }
        
        await TryExpandAsync().ConfigureAwait(false);
        
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
        
        Func<Task>? sideEffect = GetOptions().SideEffect;
        if (sideEffect != null)
        {
            await sideEffect().ConfigureAwait(false);
        }
        
        await GetPage()
            .WaitForTimeoutAsync(100)
            .ConfigureAwait(false);
        
        return item!;
    }

    public virtual async Task<string> SelectItemsAsync()
    {
        await TryExpandAsync().ConfigureAwait(false);
        
        IReadOnlyList<ILocator> selectOptions =
            await GetLocator(GetXPathItem())
                .AllAsync()
                .ConfigureAwait(false);

        foreach (ILocator selectOption in selectOptions)
        {
            await selectOption
                .CheckAsync()
                .ConfigureAwait(false);
        }

        return nameof(SelectItems.ALL);
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

    protected async Task<bool> TryExpandAsync()
    {
        var result = GetOptions().Expandable != null && GetOptions().Expandable!.Value;

        if (result)
        {
            await ExpandAsync().ConfigureAwait(false);
        }
        
        return result;
    }
}