using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reporting.AzureDevOps.Exceptions;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class WorkItemTypesRepository(
    AzureWorkItemsGateway azureWorkItemsGateway,
    AzureDevOpsOptions azureDevOpsOptions,
    IMemoryCache memoryCache,
    ILoggerFactory? loggerFactory = null)
    : BaseAzureDevOpsRepository<WorkItemType>(memoryCache, loggerFactory)
{
    private const string CacheKeyPrefix = "WorkItemType_";
    
    private readonly AzureDevOpsOptions _azureDevOpsOptions = azureDevOpsOptions
        ?? throw new ArgumentNullException(nameof(azureDevOpsOptions));
    private readonly AzureWorkItemsGateway _azureWorkItemsGateway = azureWorkItemsGateway
        ?? throw new ArgumentNullException(nameof(azureWorkItemsGateway));
    
    public async Task<WorkItemType?> GetWorkItemTypeByNameAsync(
        string workItemTypeName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workItemTypeName))
        {
            throw new WorkItemTypeNameArgumentException(nameof(workItemTypeName));
        }
        
        string cacheKey = $"{CacheKeyPrefix}{workItemTypeName}";
        return await GetItemAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }

    protected override string GetCacheKey(string itemName)
        => $"{CacheKeyPrefix}{itemName}";

    protected override string GetCacheKey(WorkItemType item)
        => GetCacheKey(item.Name);

    protected override Task<IEnumerable<WorkItemType>> LoadItemsAsync(CancellationToken cancellationToken = default)
        => _azureWorkItemsGateway.GetWorkItemTypesAsync(_azureDevOpsOptions.ProjectName!, cancellationToken);
}