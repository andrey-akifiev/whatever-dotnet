using Microsoft.Playwright;

namespace WhateverDotNet.Abstractions;

public interface IScenarioContext
{
    public IBrowser? Browser { get; }

    public Task AuthenticateAsync(UserCredential credential, Func<IScenarioContext, Task> authenticationFlow, Func<IScenarioContext, Task> testStartingPointFlow);

    public IPage? GetBrowserPage();
    public T? GetCurrentContext<T>();
    public T? GetCurrentDialog<T>();
    public T? GetCurrentPage<T>();
    public UserCredential? GetCurrentUser();
    public string? GetPermanentArtifactPrefix();
    public string? GetTemporaryArtifactPrefix();
    public T? GetTestData<T>(string key);
    public void SetCurrentContext<T>(T ctx);
    public void SetCurrentDialog<T>(T dlg);
    public void SetCurrentPage<T>(T page);
    public void SetCurrentUser(UserCredential credential);
    public void SetTestData<T>(string key, T? data);
}