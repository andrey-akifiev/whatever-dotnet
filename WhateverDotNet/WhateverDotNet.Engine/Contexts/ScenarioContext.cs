namespace WhateverDotNet.Engine.Contexts;

public class ScenarioContext
{
    private readonly string _id;

    private Dictionary<string, object>? _testData;

    public ScenarioContext()
    {
        _id = Guid.NewGuid().ToString();
    }
    
    public T? GetTestData<T>(string key)
    {
        if (_testData == null || !_testData.TryGetValue(key, out var value))
        {
            throw new KeyNotFoundException($"Test data with key '{key}' not found.");
        }

        return (T?)value;
    }
    
    public void SetTestData<T>(string key, T? data)
    {
        _testData ??= new Dictionary<string, object>();
        _testData[key] = data!;
    }
}