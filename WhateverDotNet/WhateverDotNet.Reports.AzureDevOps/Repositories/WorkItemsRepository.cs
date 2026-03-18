using System.Collections.Concurrent;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reporting.AzureDevOps.Contracts;
using WhateverDotNet.Reporting.AzureDevOps.Exceptions;
using WhateverDotNet.Reporting.AzureDevOps.Extensions;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class WorkItemsRepository : IDisposable
{
    private readonly AzureWorkItemsGateway _azureWorkItemsGateway;
    private readonly ConcurrentDictionary<string, HashSet<(string, int)>> _cacheIndexByWorkItemType = new();
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<WorkItemsRepository>? _logger;
    private readonly Dictionary<string, Lazy<Task<WorkItemType>>> _workItemTypeSpecifications = new();

    protected readonly SemaphoreSlim LockObject = new(1, 1);

    public WorkItemsRepository(AzureWorkItemsGateway azureWorkItemsGateway,
        WorkItemTypesRepository workItemTypesRepository,
        IMemoryCache memoryCache,
        ILoggerFactory? loggerFactory = null)
    {
        _logger = loggerFactory?.CreateLogger<WorkItemsRepository>();
        _memoryCache = memoryCache
            ?? throw new ArgumentNullException(nameof(memoryCache));
        _azureWorkItemsGateway = azureWorkItemsGateway
            ?? throw new ArgumentNullException(nameof(azureWorkItemsGateway));

        ArgumentNullException.ThrowIfNull(workItemTypesRepository);
        _workItemTypeSpecifications.Add(
            WorkItemTypes.TestCase,
            new Lazy<Task<WorkItemType>>(async () =>
                await workItemTypesRepository.GetWorkItemTypeByNameAsync(WorkItemTypes.TestCase)
                    ?? throw new WorkItemTypeNotFoundException(WorkItemTypes.TestCase),
                LazyThreadSafetyMode.ExecutionAndPublication));
    }

    public virtual void Dispose()
    {
        LockObject.Dispose();
    }

    public async Task<WorkItem> CreateWorkItemAsync(
        string projectName,
        string workItemType,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken = default)
    {
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        
        try
        {
            IReadOnlyDictionary<string, object?> workItemValues =
                await FilterNonActualValuesAsync(workItemType, values).ConfigureAwait(false);

            WorkItem workItem = await _azureWorkItemsGateway
                .CreateWorkItemAsync(
                    projectName,
                    workItemType,
                    ConvertToJsonAddDocument(workItemValues),
                    cancellationToken)
                .ConfigureAwait(false);
            
            _logger?.LogDebug(
                "Created work item of type '{WorkItemType}' with ID {WorkItemId}",
                workItemType,
                workItem.Id);
            
            AddToCache(workItem);

            return workItem;
        }
        finally
        {
            LockObject.Release();
        }
    }

    public async Task<WorkItem> GetWorkItemAsync(
        int workItemId,
        string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        if (TryGetCachedWorkItem(WorkItemTypes.TestCase, workItemId, out WorkItem? cachedWorkItem))
        {
            // TryGetCachedWorkItem ensures that cachedWorkItem is not null,
            // but the compiler doesn't recognize that, hence the null-forgiving operator.
            return cachedWorkItem!;
        }
        
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            if (TryGetCachedWorkItem(WorkItemTypes.TestCase, workItemId, out cachedWorkItem))
            {
                return cachedWorkItem!;
            }

            WorkItem workItem = await _azureWorkItemsGateway
                .GetWorkItemByIdAsync(
                    workItemId,
                    fields,
                    cancellationToken)
                .ConfigureAwait(false);
            
            AddToCache(workItem);

            return workItem;
        }
        finally
        {
            LockObject.Release();
        }
    }
    
    public async Task<WorkItem> GetWorkItemAsync(
        string workItemType,
        int workItemId,
        string[]? fields = null,
        CancellationToken cancellationToken = default)
    {
        if (TryGetCachedWorkItem(workItemType, workItemId, out WorkItem? cachedWorkItem))
        {
            // TryGetCachedWorkItem ensures that cachedWorkItem is not null,
            // but the compiler doesn't recognize that, hence the null-forgiving operator.
            return cachedWorkItem!;
        }
        
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            if (TryGetCachedWorkItem(workItemType, workItemId, out cachedWorkItem))
            {
                return cachedWorkItem!;
            }

            WorkItem workItem = await _azureWorkItemsGateway
                .GetWorkItemByIdAsync(
                    workItemId,
                    fields,
                    cancellationToken)
                .ConfigureAwait(false);
            
            AddToCache(workItem);

            return workItem;
        }
        finally
        {
            LockObject.Release();
        }
    }

    public async Task<IEnumerable<WorkItem>> GetWorkItemsAsync(
        string projectName,
        string workItemType,
        string[] fields,
        string? areaPath = null,
        CancellationToken cancellationToken = default)
    {
        if (_cacheIndexByWorkItemType.TryGetValue(workItemType, out HashSet<(string, int)>? index)
            && index.Count != 0)
        {
            return index
                .Select(item => (WorkItem)_memoryCache.Get(item)!);
        }
        
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            if (_cacheIndexByWorkItemType.TryGetValue(workItemType, out index)
                && index.Count != 0)
            {
                return index
                    .Select(item => (WorkItem)_memoryCache.Get(item)!);
            }

            IEnumerable<WorkItem> workItems = await LoadItemsAsync(
                projectName,
                workItemType,
                areaPath,
                fields,
                cancellationToken);
            
            // ReSharper disable once PossibleMultipleEnumeration
            AddRangeToCache(workItems);

            // ReSharper disable once PossibleMultipleEnumeration
            return workItems;
        }
        finally
        {
            LockObject.Release();
        }
    }

    public async Task<WorkItem> UpdateWorkItemAsync(
        string projectName,
        string workItemType,
        int workItemId,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetCachedWorkItem(workItemType, workItemId, out WorkItem? cachedWorkItem))
        {
            cachedWorkItem = await GetWorkItemAsync(
                    workItemId,
                    fields: await FilterNonActualFieldsAsync(
                            workItemType, 
                            values
                                .Select(v => v.Key)
                                .ToArray())
                        .ConfigureAwait(false),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        
        IDictionary<string, object?>? actualValues = cachedWorkItem!.Fields;

        if (AreValuesEqual(values, actualValues))
        {
            // Values are the same as current, no update needed, so we can skip the update call to Azure DevOps.
            return cachedWorkItem;
        }
        
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        
        try
        {
            IReadOnlyDictionary<string, object?> workItemValues =
                await FilterNonActualValuesAsync(workItemType, values).ConfigureAwait(false);

            WorkItem workItem = await _azureWorkItemsGateway
                .CreateWorkItemAsync(
                    projectName,
                    workItemType,
                    ConvertToJsonAddDocument(workItemValues),
                    cancellationToken)
                .ConfigureAwait(false);
            
            AddToCache(workItem);

            return workItem;
        }
        finally
        {
            LockObject.Release();
        }
    }

    private JsonPatchDocument ConvertToJsonAddDocument(IReadOnlyDictionary<string, object?> values)
        => ConvertToJsonPatchDocument(Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Add, values);
    
    private JsonPatchDocument ConvertToJsonUpdateDocument(IReadOnlyDictionary<string, object?> values)
        => ConvertToJsonPatchDocument(Microsoft.VisualStudio.Services.WebApi.Patch.Operation.Replace, values);
    
    private JsonPatchDocument ConvertToJsonPatchDocument(
        Microsoft.VisualStudio.Services.WebApi.Patch.Operation patchOperation,
        IReadOnlyDictionary<string, object?> values)
    {
        JsonPatchDocument document = new();
        document.AddRange(
            values
                .Select(value =>
                    new JsonPatchOperation
                    {
                        Operation = patchOperation,
                        Path = $"/fields/{value.Key}",
                        Value = value.Value,
                    }));
        return document;
    }

    protected async Task<string[]> FilterNonActualFieldsAsync(
        string workItemType,
        IEnumerable<string> fieldNames)
    {
        IEnumerable<string> allActualFields =
            (await GetWorkItemTypeSpecificationAsync(workItemType)
                .ConfigureAwait(false))
                .Fields
                .Select(f => f.ReferenceName);
        return allActualFields
            .Intersect(fieldNames)
            .ToArray();
    }
    
    protected async Task<IReadOnlyDictionary<string, object?>> FilterNonActualValuesAsync(
        string workItemType,
        IReadOnlyDictionary<string, object?> values)
    {
        string[] actualFields = 
            await FilterNonActualFieldsAsync(
                    workItemType,
                    values
                        .Select(v => v.Key)
                        .ToArray())
                .ConfigureAwait(false);

        return values
            .Where(v => actualFields.Contains(v.Key))
            .ToDictionary(v => v.Key, v => v.Value);
    }

    protected async Task<WorkItemType> GetWorkItemTypeSpecificationAsync(string workItemTypeName)
    {
        return await _workItemTypeSpecifications[workItemTypeName].Value;
    }
    
    protected Task<IEnumerable<WorkItem>> LoadItemsAsync(
        string projectName,
        string workItemType,
        string? areaPath = null,
        string[]? fieldsToInclude = null,
        CancellationToken cancellationToken = default)
    {
        return _azureWorkItemsGateway.GetAllWorkItemsAsync(
            projectName,
            workItemType,
            areaPath,
            fieldsToInclude,
            cancellationToken);
    }

    private void AddRangeToCache(IEnumerable<WorkItem> workItems)
    {
        foreach (WorkItem workItem in workItems)
        {
            AddToCache(workItem);
        }
    }

    private void AddToCache(WorkItem workItem)
    {
        string workItemType = workItem.GetWorkItemType();
        int workItemId = workItem.Id
            ?? throw new InvalidOperationException("In this context, the work item must have a valid ID.");
        AddToCache(workItemType, workItemId, workItem);
    }

    private void AddToCache(string workItemType, int workItemId, WorkItem workItem)
    {
        (string type, int id) cacheKey = GetCacheKey(workItemType, workItemId);
        
        _memoryCache.Set(cacheKey, workItem);
        
        HashSet<(string, int)> set =
            _cacheIndexByWorkItemType.GetOrAdd(workItemType, _ => new HashSet<(string, int)>());
        
        lock (set)
        {
            set.Add(cacheKey);
        }
    }
    
    private bool AreValuesEqual(IReadOnlyDictionary<string, object?>? values1, IDictionary<string, object?>? values2)
    {
        if (values1 is null && values2 is null) return true;
        if (values1 is null || values2 is null) return false;
        if (values1.Count != values2.Count) return false;

        foreach (KeyValuePair<string, object?> kvp in values1)
        {
            if (!values2.TryGetValue(kvp.Key, out object? value2)) return false;
            if (kvp.Value is null && value2 is null) continue;
            
            // TODO: Consider using a more robust equality check,
            // especially for complex field values (e.g., multi-value fields, HTML fields, etc.) -- aa
            if (kvp.Value is null || value2 is null || !kvp.Value.Equals(value2)) 
            {
                return false;
            }
        }

        return true;
    }
    
    private (string type, int id) GetCacheKey(string workItemType, int workItemId)
        => (workItemType, workItemId);
    
    private (string type, int id) GetCacheKey(WorkItem workItem)
        => GetCacheKey(workItem.GetWorkItemType(), (int)workItem.Id!);
    
    private bool TryGetCachedWorkItem(string workItemType, int workItemId, out WorkItem? workItem)
    {
        if (_cacheIndexByWorkItemType.TryGetValue(workItemType, out HashSet<(string, int)>? index)
            && index.Contains((workItemType, workItemId))
            && _memoryCache.TryGetValue((workItemType, workItemId), out WorkItem? cachedWorkItem)
            && cachedWorkItem != null)
        {
            workItem = cachedWorkItem;
            return true;
        }

        workItem = null;
        return false;
    }
    
    // public async Task<TestCase> GetTestCaseByIdAsync(
    //     TestPlan testPlan,
    //     TestSuite testSuite,
    //     int testCaseId,
    //     CancellationToken cancellationToken = default)
    // {
    //     var cacheKeyTemplate = $"TestCase_{testPlan.Id}_{testSuite.Id}_{{0}}";
    //     var requestedCacheKey = string.Format(cacheKeyTemplate, testCaseId);
    //
    //     if (_memoryCache.TryGetValue(key: requestedCacheKey, out TestCase? cachedTestCase))
    //     {
    //         if (cachedTestCase != null)
    //         {
    //             return cachedTestCase;
    //         }
    //     }
    //
    //     await _lockObject
    //         .WaitAsync(cancellationToken)
    //         .ConfigureAwait(false);
    //     try
    //     {
    //         if (_memoryCache.TryGetValue(key: requestedCacheKey, out TestCase? cachedTestCase2))
    //         {
    //             if (cachedTestCase2 != null)
    //             {
    //                 return cachedTestCase2;
    //             }
    //         }
    //
    //         IEnumerable<TestCase> testCases = await _azureDevOpsService
    //             .GetTestCaseByTestSuiteAsync(testPlan, testSuite, cancellationToken)
    //             .ConfigureAwait(false);
    //
    //         foreach (TestCase testCase in testCases)
    //         {
    //             var key = string.Format(cacheKeyTemplate, testCase.workItem.Id);
    //             _memoryCache.GetOrCreate(key, (entry) => testCase);
    //         }
    //
    //         if (_memoryCache.TryGetValue(requestedCacheKey, out TestCase? cachedTestCase3))
    //         {
    //             return cachedTestCase3!;
    //         }
    //
    //         var err = new TestCaseNotFoundEception(_options.Project, testCaseId);
    //
    //         // TODO: Log error? -- aa
    //         throw err;
    //     }
    //     finally
    //     {
    //         _lockObject.Release();
    //     }
    // }
    //
    // public async Task<TestPlan> GetTestPlanByNameAsync(
    //     string testPlanName,
    //     CancellationToken cancellationToken = default)
    // {
    //     var key = $"TestPlan_{testPlanName}";
    //
    //     if (_memoryCache.TryGetValue(key: key, out TestPlan? cachedTestPlan))
    //     {
    //         if (cachedTestPlan != null)
    //         {
    //             return cachedTestPlan;
    //         }
    //     }
    //
    //     await _lockObject.WaitAsync(cancellationToken);
    //     try
    //     {
    //         return (await _memoryCache
    //             .GetOrCreateAsync(
    //                 key: key,
    //                 factory: async entry =>
    //                 {
    //                     return await _azureDevOpsService
    //                         .GetTestPlanByNameAsync(testPlanName, cancellationToken)
    //                         .ConfigureAwait(false);
    //                 })
    //             .ConfigureAwait(false))!;
    //     }
    //     finally
    //     {
    //         _lockObject.Release();
    //     }
    // }
    //
    // public async Task<IEnumerable<TestPoint>> GetTestPointsAsync(
    //     TestPlan testPlan,
    //     TestSuite testSuite,
    //     TestCase testCase,
    //     CancellationToken cancellationToken = default(CancellationToken))
    // {
    //     return await _azureDevOpsService
    //         .GetTestPointsAsync(testPlan, testSuite, testCase, cancellationToken)
    //         .ConfigureAwait(false);
    // }
    //
    // public async Task<IEnumerable<TestPoint>> GetTestPointsAsync(
    //     TestPlan testPlan,
    //     TestSuite testSuite,
    //     int testCaseId,
    //     CancellationToken cancellationToken = default(CancellationToken))
    // {
    //     return await _azureDevOpsService
    //         .GetTestPointsAsync(testPlan, testSuite, testCaseId, cancellationToken)
    //         .ConfigureAwait(false);
    // }
    //
    // public async Task<TestSuite> GetTestSuiteByNameAsync(TestPlan testPlan, string testSuiteName, CancellationToken cancellationToken = default(CancellationToken))
    // {
    //     var key = $"TestSuite_{testPlan.Id}_{testSuiteName}";
    //
    //     if (_memoryCache.TryGetValue(key: key, out TestSuite? cachedTestSuite))
    //     {
    //         if (cachedTestSuite != null)
    //         {
    //             return cachedTestSuite;
    //         }
    //     }
    //
    //     await _lockObject.WaitAsync(cancellationToken);
    //     try
    //     {
    //         return (await _memoryCache
    //             .GetOrCreateAsync(
    //                 key: key,
    //                 factory: async entry =>
    //                 {
    //                     return await _azureDevOpsService
    //                         .GetTestSuiteByNameAsync(testPlan, testSuiteName, cancellationToken)
    //                         .ConfigureAwait(false);
    //                 })
    //             .ConfigureAwait(false))!;
    //     }
    //     finally
    //     {
    //         _lockObject.Release();
    //     }
    // }
}