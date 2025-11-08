namespace WhateverDotNet.Components;

public class TimeoutOptions
{
    /// <summary>
    /// Maximum time in milliseconds. Defaults to `0` - no timeout. The default value can be changed via `actionTimeout`
    /// option in the config, or by using the
    /// [browserContext.setDefaultTimeout(timeout)](https://playwright.dev/docs/api/class-browsercontext#browser-context-set-default-timeout)
    /// or [page.setDefaultTimeout(timeout)](https://playwright.dev/docs/api/class-page#page-set-default-timeout) methods.
    /// </summary>
    public int? TimeoutInMilliseconds { get; set;  }
}