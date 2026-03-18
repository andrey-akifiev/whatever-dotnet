using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public abstract class BaseAzureDevOpsRepository<TItem>(
    IMemoryCache cache,
    ILoggerFactory? loggerFactory = null)
    : BaseAzureDevOpsRepository(cache, loggerFactory)
{
    public virtual async Task<TItem?> GetItemAsync(string cacheKey, CancellationToken cancellationToken = default)
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
            await UpdateCacheAsync(cancellationToken).ConfigureAwait(false);
            return Cache.Get<TItem?>(cacheKey);
        }
        finally
        {
            LockObject.Release();
        }
    }
    
    protected virtual async Task UpdateCacheAsync(CancellationToken cancellationToken = default)
    {
        Logger?.LogTrace($"{this.GetType().Name}.{nameof(UpdateCacheAsync)} called.");
        IEnumerable<TItem> items = await LoadItemsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        UpdateCache(items);
    }
    
    protected virtual void UpdateCache(IEnumerable<TItem> items)
    {
        foreach (TItem item in items)
        {
            string cacheKey = GetCacheKey(item);
            Cache.Set(cacheKey, item);
        }
    }

    protected abstract string GetCacheKey(string itemName);

    protected abstract string GetCacheKey(TItem item);
    
    protected abstract Task<IEnumerable<TItem>> LoadItemsAsync(CancellationToken cancellationToken = default);
}