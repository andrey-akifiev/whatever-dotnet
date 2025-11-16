using Microsoft.Playwright;

using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Engine.Contexts;

public class ScenarioContext : IScenarioContext
{
    private readonly string _id;

    private Dictionary<string, object>? _testData;

    protected TestRunContext _testRunContext;

    public ScenarioContext()
    {
        _id = Guid.NewGuid().ToString();
    }

    public IBrowser? Browser => _testRunContext.Browser;

    public async Task AuthenticateAsync(
        UserCredential credential,
        Func<IScenarioContext, Task> authenticationFlow,
        Func<IScenarioContext, Task> testStartingPointFlow)
    {
        await _testRunContext
            .BrowserContextManager
            .SetOrCreateSessionAsync(this, credential, authenticationFlow, testStartingPointFlow)
            .ConfigureAwait(false);
    }

    public IPage? GetBrowserPage()
    {
        return GetTestData<IPage>(Constants.CurrentBrowserPageVariable);
    }

    public T? GetCurrentContext<T>()
    {
        return GetTestData<T>(Constants.CurrentContextVariable);
    }

    public T? GetCurrentDialog<T>()
    {
        return GetTestData<T>(Constants.CurrentDialogVariable);
    }

    public T? GetCurrentPage<T>()
    {
        return GetTestData<T>(Constants.CurrentPageVariable);
    }

    public UserCredential? GetCurrentUser()
    {
        return GetTestData<UserCredential>(Constants.CurrentUserVariable);
    }

    public string? GetPermanentArtifactPrefix()
    {
        return GetTestData<string>(Constants.PermanentArtifactPrefix);
    }

    public string? GetTemporaryArtifactPrefix()
    {
        return GetTestData<string>(Constants.TemporaryArtifactPrefix);
    }

    public T? GetTestData<T>(string key)
    {
        if (_testData == null || !_testData.TryGetValue(key, out var value))
        {
            throw new KeyNotFoundException($"Test data with key '{key}' not found.");
        }

        return (T?)value;
    }

    public virtual TestRunContext GetTestRunContext()
    {
        return _testRunContext;
    }

    public void SetCurrentContext<T>(T? ctx)
    {
        SetTestData(Constants.CurrentContextVariable, ctx);
    }

    public void SetCurrentDialog<T>(T? dlg)
    {
        SetCurrentContext(dlg);
        SetTestData(Constants.CurrentDialogVariable, dlg);
    }

    public void SetCurrentPage<T>(T? page)
    {
        SetCurrentContext(page);
        SetTestData(Constants.CurrentPageVariable, page);
    }
    
    public void SetCurrentUser(UserCredential credential)
    {
        SetTestData(Constants.CurrentUserVariable, credential);
    }

    public void SetTestData<T>(string key, T? data)
    {
        _testData ??= new Dictionary<string, object>();
        _testData[key] = data!;
    }

    public virtual void SetTestRunContext(TestRunContext context)
    {
        _testRunContext = context;
    }
}