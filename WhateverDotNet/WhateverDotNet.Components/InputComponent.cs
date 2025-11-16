using Microsoft.Playwright;

using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components;

public class InputComponent : BaseComponent<InputComponentOptions>
{
    public InputComponent(IScenarioContext scenarioContext, InputComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    public override InputComponentOptions GetOptions()
    {
        return _options;
    }
    
    public override async Task<string> TypeAsync(string text, ForceOptions? options = null)
    {
        ILocator inputLocator = GetLocator($"{GetXPathComponent()}{GetOptions().XPathInput}");
        await inputLocator.ClickAsync().ConfigureAwait(false);
        await inputLocator.ClearAsync();
        await inputLocator.FillAsync(text);

        return text;
    }
}