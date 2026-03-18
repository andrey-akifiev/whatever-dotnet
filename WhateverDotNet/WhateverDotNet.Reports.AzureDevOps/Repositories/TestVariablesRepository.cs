using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class TestVariablesRepository(
    AzureTestPlansGateway azureTestPlansGateway,
    AzureDevOpsOptions azureDevOpsOptions,
    IMemoryCache cache,
    ILoggerFactory? loggerFactory = null)
    : BaseAzureDevOpsRepository<TestVariable>(cache, loggerFactory)
{
    private const string CacheKeyPrefix = "TestVariable_";
    
    private readonly AzureDevOpsOptions _azureDevOpsOptions = azureDevOpsOptions
        ?? throw new ArgumentNullException(nameof(azureDevOpsOptions));
    private readonly AzureTestPlansGateway _azureTestPlansGateway = azureTestPlansGateway
        ?? throw new ArgumentNullException(nameof(azureTestPlansGateway));
    
    public async Task<TestVariable> CreateTestVariableAsync(
        string variableName,
        IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        
        try
        {
            TestVariable response = await _azureTestPlansGateway
                .CreateTestVariableAsync(
                    _azureDevOpsOptions.ProjectName!,
                    variableName,
                    null,
                    values,
                    cancellationToken)
                .ConfigureAwait(false);
            
            Cache.Set(GetCacheKey(response), response);
            
            return response; 
        }
        finally
        {
            LockObject.Release();
        }
    }
    
    public async Task<TestVariable?> GetTestVariableByNameAsync(
        string variableName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(variableName))
        {
            throw new ArgumentException(
                "Test variable name cannot be null or whitespace.",
                nameof(variableName));
        }
        
        string cacheKey = GetCacheKey(variableName);
        return await GetItemAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TestVariable?> UpdateTestVariableAsync(
        string oldName,
        string newName,
        IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        string oldKey = GetCacheKey(oldName);
        string newKey = GetCacheKey(newName);
        
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        
        try
        {
            // Old value is not found, so we create a new variable with the new name and values
            if (!Cache.TryGetValue(oldName, out TestVariable? oldTestVariable)
                || oldTestVariable == null)
            {
                return await CreateTestVariableAsync(
                    newName,
                    values,
                    cancellationToken).ConfigureAwait(false);
            }
            
            // If the old variable has the same name and values as the new variable
            // we can return the old variable without making an API call
            if (oldTestVariable.Name == newName
                && (oldTestVariable.Values == null && values == null
                || values != null
                && !(oldTestVariable.Values?.Except(values)).Any()))
            {
                return oldTestVariable;
            }
            
            // Update the variable with the new name and values
            TestVariable updateResponse = await _azureTestPlansGateway
                .UpdateTestVariableAsync(
                    _azureDevOpsOptions.ProjectName!,
                    oldTestVariable.Id,
                    newName,
                    null,
                    values,
                    cancellationToken)
                .ConfigureAwait(false);

            // Update the cache with the new variable
            if (oldName != newName)
            {
                Cache.Remove(GetCacheKey(oldName));
            }
            
            Cache.Set(GetCacheKey(newName), updateResponse);
            
            return updateResponse;
        }
        finally
        {
            LockObject.Release();
        }
    }
    
    protected override string GetCacheKey(string itemName)
        => $"{CacheKeyPrefix}{itemName}";

    protected override string GetCacheKey(TestVariable item)
        => GetCacheKey(item.Name);

    protected override Task<IEnumerable<TestVariable>> LoadItemsAsync(CancellationToken cancellationToken = default)
        => _azureTestPlansGateway.GetTestVariablesAsync(_azureDevOpsOptions.ProjectName!, cancellationToken);
}