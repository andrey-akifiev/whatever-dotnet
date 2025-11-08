using Microsoft.Playwright;

namespace WhateverDotNet.Abstractions;

public interface IScenarioContext
{
    public IPage GetBrowserPage();
    public T? GetTestData<T>(string key);
    public void SetTestData<T>(string key, T? data);
}