using Microsoft.Playwright;

using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;

namespace WhateverDotNet.Components.Buttons;

public class DownloadButtonComponent : ButtonComponent
{
    public DownloadButtonComponent(IScenarioContext scenarioContext, ButtonComponentOptions options)
        : base(scenarioContext, options)
    {
    }
    
    public override async Task ClickAsync(LocatorClickOptions? options = null)
    {
        Task<IDownload> downloadTask = GetPage().WaitForDownloadAsync();
        
        await base.ClickAsync(options).ConfigureAwait(false);
        
        IDownload download = await downloadTask.ConfigureAwait(false);
        string? downloadDirectory = _scenarioContext.GetTestData<string>("DOWNLOAD_DIRECTORY");

        if (string.IsNullOrWhiteSpace(downloadDirectory))
        {
            throw new OptionNotDefinedException("DOWNLOAD_DIRECTORY");
        }

        string downloadPath = Path.Join(downloadDirectory, download.SuggestedFilename);
        
        await download.SaveAsAsync(downloadPath).ConfigureAwait(false);
        
        _scenarioContext.SetTestData("CURRENT_FILE", downloadPath);

        await OnClickAsync().ConfigureAwait(false);
    }
}