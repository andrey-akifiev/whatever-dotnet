using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class TestConfigurationsRepository(
    AzureTestPlansGateway azureTestPlansGateway,
    IMemoryCache cache,
    AzureDevOpsOptions azureDevOpsOptions,
    ILoggerFactory? loggerFactory = null)
    : BaseAzureDevOpsRepository<TestConfiguration>(cache, loggerFactory)
{
    private const string CacheKeyPrefix = "TestConfiguration_";
    
    private readonly AzureTestPlansGateway _azureTestPlansGateway = azureTestPlansGateway
        ?? throw new ArgumentNullException(nameof(azureTestPlansGateway));
    private readonly AzureDevOpsOptions _azureDevOpsOptions = azureDevOpsOptions
        ?? throw new ArgumentNullException(nameof(azureDevOpsOptions));
    
    public async Task<TestConfiguration> CreateTestConfigurationAsync(
        string configurationName,
        IReadOnlyDictionary<string, string>? values = null,
        CancellationToken cancellationToken = default)
    {
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            TestConfiguration response = await _azureTestPlansGateway
                .CreateTestConfigurationAsync(
                    _azureDevOpsOptions.ProjectName!,
                    configurationName,
                    values: values,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            string cacheKey = GetCacheKey(response);
            Cache.Set(cacheKey, response);
            return response;
        }
        finally
        {
            LockObject.Release();
        }
    }
    
    public async Task<TestConfiguration?> GetTestConfigurationByNameAsync(
        string configurationName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(configurationName))
        {
            throw new ArgumentException(
                "Test configuration name cannot be null or whitespace.",
                nameof(configurationName));
        }
        
        string cacheKey = GetCacheKey(configurationName);
        return await GetItemAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }
    
    protected override string GetCacheKey(string itemName)
        => $"{CacheKeyPrefix}{itemName}";

    protected override string GetCacheKey(TestConfiguration item)
        => GetCacheKey(item.Name);

    protected override Task<IEnumerable<TestConfiguration>> LoadItemsAsync(CancellationToken cancellationToken = default)
        => _azureTestPlansGateway.GetTestConfigurationsAsync(_azureDevOpsOptions.ProjectName!, cancellationToken);
}