using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public abstract class BaseAzureDevOpsRepository<TParameters, TItem>(
    IMemoryCache cache,
    ILoggerFactory? loggerFactory = null)
    : BaseAzureDevOpsRepository(cache, loggerFactory)
{
    public virtual async Task<TItem?> GetItemAsync(
        string cacheKey,
        TParameters parameters,
        CancellationToken cancellationToken = default)
    {
        if (Cache.TryGetValue(cacheKey, out TItem? cachedItem))
        {
            return cachedItem;
        }
        
        await LockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        
        try
        {
            await UpdateCacheAsync(parameters, cancellationToken).ConfigureAwait(false);
            return Cache.Get<TItem?>(cacheKey);
        }
        finally
        {
            LockObject.Release();
        }
    }
    
    protected virtual async Task UpdateCacheAsync(
        TParameters parameters,
        CancellationToken cancellationToken = default)
    {
        Logger?.LogTrace($"{this.GetType().Name}.{nameof(UpdateCacheAsync)} called.");
        IEnumerable<TItem> items = await LoadItemsAsync(parameters, cancellationToken).ConfigureAwait(false);
        UpdateCache(parameters, items);
    }
    
    protected virtual void UpdateCache(TParameters parameters, IEnumerable<TItem> items)
    {
        foreach (TItem item in items)
        {
            string cacheKey = GetCacheKey(parameters, item);
            Cache.Set(cacheKey, item);
        }
    }

    protected abstract string GetCacheKey(TParameters parameters, string itemName);

    protected abstract string GetCacheKey(TParameters parameters, TItem item);
    
    protected abstract Task<IEnumerable<TItem>> LoadItemsAsync(
        TParameters parameters,
        CancellationToken cancellationToken = default);
}