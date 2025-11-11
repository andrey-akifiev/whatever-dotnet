using Microsoft.Playwright;
using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components.Buttons;

public class UploadButtonComponent : ButtonComponent
{
    public UploadButtonComponent(IScenarioContext scenarioContext, ButtonComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    public override async Task ClickAsync(LocatorClickOptions? options)
    {
        var fileChooser = await GetPage()
            .RunAndWaitForFileChooserAsync(async () =>
            {
                await base
                    .ClickAsync(options)
                    .ConfigureAwait(false);
            }).ConfigureAwait(false);
        await fileChooser
            .SetFilesAsync("./downloads/" + "undefined.xlsx")
            .ConfigureAwait(false);
        
        await OnClickAsync().ConfigureAwait(false);
    }
}