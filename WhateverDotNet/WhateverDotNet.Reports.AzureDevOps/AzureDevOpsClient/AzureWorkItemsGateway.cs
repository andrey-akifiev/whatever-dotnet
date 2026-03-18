using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

using WhateverDotNet.Reporting.AzureDevOps.Contracts;

namespace WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

public class AzureWorkItemsGateway(AzureDevOpsService azureDevOpsService, ILoggerFactory? loggerFactory)
    : BaseAzureDevOpsGateway<WorkItemTrackingHttpClient>(azureDevOpsService)
{
    private const int QueryPageSizeLimit = 19999;
    
    private readonly ILogger<AzureWorkItemsGateway>? _logger = loggerFactory?.CreateLogger<AzureWorkItemsGateway>();
    
    public async Task<WorkItem> CreateWorkItemAsync(
        string projectName,
        string workItemType,
        JsonPatchDocument document,
        CancellationToken cancellationToken = default)
    {
        return await Client
            .CreateWorkItemAsync(document, projectName, workItemType, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<WorkItem>> GetAllWorkItemsAsync(
        string projectName,
        string workItemType,
        string? areaPath = null,
        string[]? fieldsToInclude = null,
        CancellationToken cancellationToken = default)
    {
        string query = $"""
                        SELECT [System.Id]
                        FROM WorkItems
                        WHERE [System.TeamProject] = '{projectName}'
                        AND [System.WorkItemType] = '{workItemType}'
                        """;

        if (!string.IsNullOrWhiteSpace(areaPath))
        {
            query += $" AND [System.AreaPath] UNDER '{areaPath}'";
        }
        
        return await GetAllWorkItemsByQueryAsync(
                projectName,
                query,
                fieldsToInclude,
                cancellationToken)
            .ConfigureAwait(false);
    }
    
    public async Task<IEnumerable<WorkItem>> GetAllWorkItemsByQueryAsync(
        string projectName,
        string query,
        IEnumerable<string>? fieldsToInclude = null,
        CancellationToken cancellationToken = default)
    {
        bool morePages = true;
        int lastWorkItemId = 0;

        List<int> workItemIds = new();
        while (morePages)
        {
            string pageQuery = $"{query} AND [{WorkItemStandardFields.Id}] > {lastWorkItemId} ORDER BY [{WorkItemStandardFields.Id}] ASC";
            IEnumerable<WorkItemReference> pageResults =
                await GetWorkItemsByQueryAsync(projectName, pageQuery, cancellationToken).ConfigureAwait(false);

            int pageResultsCount = pageResults.Count();

            if (pageResultsCount == 0)
            {
                break;
            }

            if (pageResultsCount < QueryPageSizeLimit)
            {
                morePages = false;
            }

            lastWorkItemId = pageResults.LastOrDefault()?.Id ?? int.MaxValue; // TODO: Review it -- aa

            workItemIds.AddRange(pageResults.Select(wir => wir.Id));
        }

        return await GetWorkItemsByIdsAsync(
                workItemIds,
                fieldsToInclude,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<WorkItem> GetWorkItemByIdAsync(
        int workItemId,
        string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        return await Client
            .GetWorkItemAsync(
                workItemId,
                fields: fields,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<WorkItem>> GetWorkItemsByIdsAsync(
        IEnumerable<int> workItemIds,
        IEnumerable<string>? fieldsToInclude = null,
        CancellationToken cancellationToken = default)
    {
        const int batchSize = 200;
        const int maxParallel = 5;
        
        using SemaphoreSlim semaphore = new(maxParallel);
        IEnumerable<Task<IEnumerable<WorkItem>>> tasks =
            workItemIds
                .Chunk(batchSize)
                .Select(async batch =>
                {
                    await semaphore
                        .WaitAsync(cancellationToken)
                        .ConfigureAwait(false);
                    try
                    {
                        return await GetWorkItemsBatchAsync(
                            batch,
                            fieldsToInclude,
                            cancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
        
        IEnumerable<WorkItem>[] results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.SelectMany(wir => wir);
    }
    
    public async Task<IEnumerable<WorkItemReference>> GetWorkItemsByQueryAsync(
        string projectName,
        string query,
        CancellationToken cancellationToken = default)
    {
        Wiql wiql = new(){ Query = query };
        _logger?.LogTrace("Getting work items by query: '{Query}'.", query);
        return (await Client
                .QueryByWiqlAsync(
                    wiql,
                    project: projectName,
                    top: QueryPageSizeLimit,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false))
            .WorkItems;
    }

    public async Task<WorkItemType> GetWorkItemTypeAsync(
        string projectName,
        string workItemTypeName,
        CancellationToken cancellationToken = default)
        => (await GetWorkItemTypesAsync(projectName, cancellationToken)
                .ConfigureAwait(false))
            .First(wit => wit.Name == workItemTypeName);

    public async Task<IEnumerable<WorkItemType>> GetWorkItemTypesAsync(
        string projectName,
        CancellationToken cancellationToken = default)
        => await Client
            .GetWorkItemTypesAsync(projectName, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

    public async Task<WorkItemTypeFieldInstance> GetWorkItemTypeFieldAsync(
        string projectName,
        string workItemTypeName,
        string fieldName,
        CancellationToken cancellationToken = default)
    {
        return await Client
            .GetWorkItemTypeFieldAsync(
                projectName,
                workItemTypeName,
                fieldName,
                expand: WorkItemTypeFieldsExpandLevel.All,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    
    public async Task<IEnumerable<WorkItem>> GetWorkItemRevisionsAsync(
        int workItemId,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return await Client
            .GetRevisionsAsync(
                workItemId,
                expand: WorkItemExpand.None,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    
    public async Task<WorkItem> UpdateWorkItemAsync(
        string projectName,
        int workItemId,
        JsonPatchDocument document,
        CancellationToken cancellationToken = default)
    {
        return await Client
            .UpdateWorkItemAsync(
                document,
                projectName,
                workItemId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<IEnumerable<WorkItem>> GetWorkItemsBatchAsync(
        int[] workItemIds,
        IEnumerable<string>? fieldsToInclude = null,
        CancellationToken cancellationToken = default)
    {
        _logger?.LogTrace(
            "Getting work items batch with IDs: '{WorkItemIds}'.",
            string.Join(", ", workItemIds));
        return await Client
            .GetWorkItemsBatchAsync(
                new WorkItemBatchGetRequest
                {
                    Fields = fieldsToInclude,
                    Ids = workItemIds,
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}