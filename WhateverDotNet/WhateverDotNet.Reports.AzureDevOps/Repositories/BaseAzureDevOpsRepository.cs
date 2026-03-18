using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public abstract class BaseAzureDevOpsRepository(
    IMemoryCache cache,
    ILoggerFactory? loggerFactory = null)
    : IDisposable
{
    protected readonly IMemoryCache Cache = cache
        ?? throw new ArgumentNullException(nameof(cache));
    protected readonly ILogger<BaseAzureDevOpsRepository>? Logger =
        loggerFactory?.CreateLogger<BaseAzureDevOpsRepository>();

    protected readonly SemaphoreSlim LockObject = new(1, 1);
    
    public virtual void Dispose()
    {
        LockObject.Dispose();
    }
}