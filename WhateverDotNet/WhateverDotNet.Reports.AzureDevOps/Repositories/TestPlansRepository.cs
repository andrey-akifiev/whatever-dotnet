using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class TestPlansRepository(
    AzureTestPlansGateway azureTestPlansGateway,
    AzureDevOpsOptions azureDevOpsOptions,
    IMemoryCache cache,
    ILoggerFactory? loggerFactory = null)
    : BaseAzureDevOpsRepository<TestPlan>(cache, loggerFactory)
{
    private const string CacheKeyPrefix = "TestPlan_";
    
    private readonly AzureTestPlansGateway _azureTestPlansGateway = azureTestPlansGateway
        ?? throw new ArgumentNullException(nameof(azureTestPlansGateway));
    private readonly AzureDevOpsOptions _azureDevOpsOptions = azureDevOpsOptions
        ?? throw new ArgumentNullException(nameof(azureDevOpsOptions));
    
    public async Task<TestPlan?> GetTestPlanByNameAsync(
        string testPlanName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(testPlanName))
        {
            throw new ArgumentException("Test plan name cannot be null or whitespace.", nameof(testPlanName));
        }

        string cacheKey = $"{CacheKeyPrefix}{testPlanName}";
        return await GetItemAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }

    protected override string GetCacheKey(string itemName)
        => $"{CacheKeyPrefix}{itemName}";

    protected override string GetCacheKey(TestPlan item)
        => GetCacheKey(item.Name);

    protected override Task<IEnumerable<TestPlan>> LoadItemsAsync(CancellationToken cancellationToken = default)
        => _azureTestPlansGateway.GetTestPlansAsync(_azureDevOpsOptions.ProjectName!, cancellationToken);
}