using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reports.Contracts;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class TestConfigurationsRepository(AzureDevOpsService azureDevOpsService, ILoggerFactory? loggerFactory = null)
    : IDisposable
{
    private const string CacheKeyPrefix = "TestConfiguration_";
    
    private readonly AzureDevOpsService _azureDevOpsService = azureDevOpsService
        ?? throw new ArgumentNullException(nameof(azureDevOpsService));
    private readonly ConcurrentDictionary<string, AgnosticTestRunConfiguration> _cache = new();
    private readonly SemaphoreSlim _lockObject = new(1, 1);
    private readonly ILogger<TestConfigurationsRepository>? _logger =
        loggerFactory?.CreateLogger<TestConfigurationsRepository>();
    
    private bool _isLoaded = false;
    
    public void Dispose()
    {
        _lockObject.Dispose();
    }

    public async Task<AgnosticTestRunConfiguration> CreateTestConfigurationAsync(
        string configurationName,
        IReadOnlyDictionary<string, string>? values = null,
        CancellationToken cancellationToken = default)
    {
        await _lockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        _isLoaded = false;

        try
        {
            TestConfiguration response = await _azureDevOpsService
                .CreateTestConfigurationAsync(
                    configurationName,
                    values: values,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var result = new AgnosticTestRunConfiguration
            {
                Id = response.Id.ToString(),
                Name = response.Name,
                Values = response.Values.ToDictionary(nvp => nvp.Name, nvp => nvp.Value),
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
    
    public async Task<AgnosticTestRunConfiguration?> GetTestConfigurationAsync(
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(cacheKey, out AgnosticTestRunConfiguration? cachedTestConfiguration))
        {
            return cachedTestConfiguration!;
        }

        await _lockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);
        _isLoaded = false;

        try
        {
            await LoadTestConfigurationsAsync(cancellationToken).ConfigureAwait(false);
            return _cache.GetValueOrDefault(cacheKey);
        }
        finally
        {
            _lockObject.Release();
        }
    }
    
    public async Task<AgnosticTestRunConfiguration?> GetTestConfigurationByNameAsync(
        string configurationName,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = $"{CacheKeyPrefix}{configurationName}";
        return await GetTestConfigurationAsync(cacheKey, cancellationToken).ConfigureAwait(false);
    }
    
    public async Task<IEnumerable<AgnosticTestRunConfiguration>> GetTestConfigurationsAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<AgnosticTestRunConfiguration> GetTestConfigurations()
        {
            return _cache
                .Keys
                .Where(k => k.StartsWith(CacheKeyPrefix))
                .Select(k => _cache.GetValueOrDefault(k))
                .Where(v => v != null)
                .ToArray()!;
        }

        if (_isLoaded)
        {
            return GetTestConfigurations();
        }
        
        await _lockObject
            .WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            if (_isLoaded)
            {
                return GetTestConfigurations();
            }
            
            await LoadTestConfigurationsAsync(cancellationToken).ConfigureAwait(false);
            
            return GetTestConfigurations();
        }
        finally
        {
            _lockObject.Release();
        }
    }

    private async Task LoadTestConfigurationsAsync(CancellationToken cancellationToken)
    {
        _cache.Clear();
        
        IEnumerable<TestConfiguration> testConfigurations = await _azureDevOpsService
            .GetTestConfigurationsAsync(cancellationToken)
            .ConfigureAwait(false);

        foreach (TestConfiguration testConfiguration in testConfigurations)
        {
            string cacheKey = $"{CacheKeyPrefix}{testConfiguration.Name}";
            _cache.TryAdd(cacheKey, new AgnosticTestRunConfiguration
            {
                Id = testConfiguration.Id.ToString(),
                Name = testConfiguration.Name,
                Values = testConfiguration.Values.ToDictionary(nvp => nvp.Name, nvp => nvp.Value),
            });
        }

        _isLoaded = true;
    }
}