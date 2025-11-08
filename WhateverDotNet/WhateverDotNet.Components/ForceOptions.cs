namespace WhateverDotNet.Components;

public class ForceOptions
{
    /// <summary>
    /// Whether to bypass the [actionability](https://playwright.dev/docs/actionability) checks. Defaults to `false`.
    /// </summary>
    public bool? Force { get; set; }
}