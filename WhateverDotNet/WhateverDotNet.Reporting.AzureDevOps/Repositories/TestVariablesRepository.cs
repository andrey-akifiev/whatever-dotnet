using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reporting.Contracts;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class TestVariablesRepository(AzureDevOpsService azureDevOpsService, ILoggerFactory? loggerFactory = null)
    : IDisposable
{
    private const string CacheKeyPrefix = "TestVariable_";
    
    private readonly AzureDevOpsService _azureDevOpsService = azureDevOpsService 
                                                    ?? throw new ArgumentNullException(nameof(azureDevOpsService));
    private readonly ConcurrentDictionary<string, AgnosticTestRunVariable> _cache = new();
    private readonly SemaphoreSlim _lockObject = new(1, 1);
    private readonly ILogger<TestVariablesRepository>? _logger = loggerFactory?.CreateLogger<TestVariablesRepository>();

    private bool _isLoaded = false;
    
    public void Dispose()
    {
        _lockObject.Dispose();
    }

    public async Task<AgnosticTestRunVariable> CreateTestVariableAsync(
        string variableName,
        IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        await _lockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        _isLoaded = false;
        
        try
        {
            TestVariable response = await _azureDevOpsService
                .CreateTestVariableAsync(
                    variableName,
                    null,
                    values,
                    cancellationToken)
                .ConfigureAwait(false);

            var result = new AgnosticTestRunVariable
            {
                Id = response.Id.ToString(),
                Name = response.Name,
                Values = response.Values,
            };
            
            _cache.TryAdd($"{CacheKeyPrefix}{response.Name}", result);
            
            _isLoaded = true;
            
            return result; 
        }
        finally
        {
            _lockObject.Release();
        }
    }
    
    public async Task<AgnosticTestRunVariable?> GetTestVariableAsync(
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(cacheKey, out AgnosticTestRunVariable? cachedTestVariable))
        {
            return cachedTestVariable!;
        }

        await _lockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        _isLoaded = false;

        try
        {
            await LoadTestVariablesAsync(cancellationToken).ConfigureAwait(false);
            return _cache.GetValueOrDefault(cacheKey);
        }
        finally
        {
            _lockObject.Release();
        }
    }
    
    public async Task<AgnosticTestRunVariable?> GetTestVariableByNameAsync(
        string variableName,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = $"{CacheKeyPrefix}{variableName}";
        return await GetTestVariableAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IEnumerable<AgnosticTestRunVariable>> GetTestVariablesAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<AgnosticTestRunVariable> GetTestVariables()
        {
            return _cache
                .Keys
                .Select(k => _cache.GetValueOrDefault(k))
                .Where(v => v != null)
                .ToArray()!;
        }

        if (_isLoaded)
        {
            return GetTestVariables();
        }
        
        await _lockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            if (_isLoaded)
            {
                return GetTestVariables();
            }

            _isLoaded = false;
            
            await LoadTestVariablesAsync(cancellationToken).ConfigureAwait(false);
            
            return GetTestVariables();
        }
        finally
        {
            _lockObject.Release();
        }
    }
    
    public async Task<AgnosticTestRunVariable?> UpdateTestVariableAsync(
        string oldName,
        string newName,
        IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        string oldKey = $"{CacheKeyPrefix}{oldName}";
        string newKey = $"{CacheKeyPrefix}{newName}";
        
        await _lockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        _isLoaded = false;
        
        try
        {
            if (_cache.IsEmpty)
            {
                _isLoaded = false;
                await LoadTestVariablesAsync(cancellationToken).ConfigureAwait(false);
            }

            if (!_cache.TryGetValue(oldName, out AgnosticTestRunVariable? oldTestVariable))
            {
                TestVariable response = await _azureDevOpsService
                    .CreateTestVariableAsync(
                        newName,
                        values: values,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                oldTestVariable = _cache.GetOrAdd(
                    oldKey,
                    new AgnosticTestRunVariable
                    {
                        Id = response.Id.ToString(),
                        Name = response.Name,
                        Values = response.Values,
                    });
            }
            
            if (oldTestVariable.Name == newName
                && (oldTestVariable.Values == null && values == null
                || values != null
                && !(oldTestVariable.Values?.Except(values)).Any()))
            {
                _isLoaded = true;
                return oldTestVariable;
            }
            
            TestVariable updateResponse = await _azureDevOpsService
                .UpdateTestVariableAsync(
                    int.Parse(oldTestVariable.Id!),
                    newName,
                    null,
                    values,
                    cancellationToken)
                .ConfigureAwait(false);
            
            AgnosticTestRunVariable updatedTestVariable = new()
            {
                Id = updateResponse.Id.ToString(),
                Name = updateResponse.Name,
                Values = updateResponse.Values,
            };

            if (oldName == newName)
            {
                return _cache.TryUpdate(oldKey, updatedTestVariable, oldTestVariable)
                    ? updatedTestVariable
                    : oldTestVariable;
            }
            
            _cache.TryRemove(oldKey, out _);
            
            if (!_cache.TryAdd(newKey, updatedTestVariable))
            {
                // TODO: Handle this scenario better
                _logger?.LogWarning(
                    "Failed to add updated test variable '{TestVariableName}' to cache.",
                    newName);
            }

            return updatedTestVariable;
        }
        finally
        {
            _lockObject.Release();
        }
    }
    
    private async Task LoadTestVariablesAsync(CancellationToken cancellationToken = default)
    {
        _cache.Clear();
        
        IEnumerable<TestVariable> testVariables = await _azureDevOpsService
            .GetTestVariablesAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (TestVariable testVariable in testVariables)
        {
            string cacheKey = $"{CacheKeyPrefix}{testVariable.Name}";
            var agnosticVariable = new AgnosticTestRunVariable
            {
                Id = testVariable.Id.ToString(),
                Name = testVariable.Name,
                Values = testVariable.Values,
            };
            _cache.TryAdd(cacheKey, agnosticVariable);
        }

        _isLoaded = true;
    }
}