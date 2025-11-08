using Microsoft.Playwright;

using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components.Buttons;

public class ButtonComponent : BaseComponent<ButtonComponentOptions>
{
    protected static readonly ButtonComponentOptions DefaultOptions = new ButtonComponentOptions
    {
    };
    
    public ButtonComponent(IScenarioContext scenarioContext, ButtonComponentOptions options)
        : base(scenarioContext, options)
    {
    }
    
    public override async Task ClickAsync(LocatorClickOptions? options)
    {
        await base.ClickAsync(options).ConfigureAwait(false);
        await OnClickAsync().ConfigureAwait(false);
    }

    public override ButtonComponentOptions GetOptions()
    {
        return _options;
    }

    protected virtual async Task OnClickAsync()
    {
        var onClickAction = GetOptions().OnClickAsync;
        
        if (onClickAction == null)
        {
            return;
        }

        await Task.Run(onClickAction);
    }
}