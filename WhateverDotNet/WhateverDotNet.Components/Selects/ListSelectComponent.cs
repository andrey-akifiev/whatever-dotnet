using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components.Selects;

public class ListSelectComponent : BaseSelectComponent
{
    public ListSelectComponent(IScenarioContext scenarioContext, SelectComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    public async Task FilterAsync(string text)
    {
        await GetLocator(GetXPathInput())
            .FillAsync(text)
            .ConfigureAwait(false);
    }

    public override async Task<string> SelectItemAsync(string item, ForceOptions? options = null)
    {
        await FilterAsync(item).ConfigureAwait(false);
        await GetPage()
            .WaitForTimeoutAsync(100)
            .ConfigureAwait(false); // Wait for filtering to take effect

        var isSelected = bool.Parse(await GetLocator($"{GetXPathItem(item)}/ancestor::label//input")
            .GetAttributeAsync("aria-checked")
            .ConfigureAwait(false)!);

        if (isSelected)
        {
            return item;
        }

        await GetLocator(GetXPathItem(item))
            .ClickAsync()
            .ConfigureAwait(false);
        
        return item;
    }
}