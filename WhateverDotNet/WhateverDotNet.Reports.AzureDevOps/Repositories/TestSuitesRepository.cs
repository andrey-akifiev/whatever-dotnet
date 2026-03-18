using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reporting.AzureDevOps.Exceptions;

namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class TestSuitesRepository(
    AzureTestPlansGateway azureTestPlansGateway,
    IMemoryCache cache,
    AzureDevOpsOptions azureDevOpsOptions,
    TestPlansRepository testPlansRepository,
    ILoggerFactory? loggerFactory = null)
    : BaseAzureDevOpsRepository<TestSuitesRepositoryParameters, TestSuite>(cache, loggerFactory)
{
    private const string CacheKeyPrefix = "TestSuite_";
    
    private readonly AzureDevOpsOptions _azureDevOpsOptions = azureDevOpsOptions
        ?? throw new ArgumentNullException(nameof(azureDevOpsOptions));
    private readonly AzureTestPlansGateway _azureTestPlansGateway = azureTestPlansGateway
        ?? throw new ArgumentNullException(nameof(azureTestPlansGateway));
    private readonly TestPlansRepository _testPlansRepository = testPlansRepository 
        ?? throw new ArgumentNullException(nameof(testPlansRepository));

    private async Task<TestSuite?> GetTestSuiteByNameAsync(
        string testPlanName,
        string testSuiteName,
        CancellationToken cancellationToken = default) 
    {
        if (string.IsNullOrWhiteSpace(testPlanName))
        {
            throw new TestPlanNameArgumentException(nameof(testPlanName));
        }
        
        if (string.IsNullOrWhiteSpace(testSuiteName))
        {
            throw new ArgumentException("Test suite name cannot be null or whitespace.", nameof(testSuiteName));
        }
        
        TestSuitesRepositoryParameters parameters = new(testPlanName);
        string cacheKey = GetCacheKey(parameters, testSuiteName);
        return await GetItemAsync(cacheKey, parameters, cancellationToken).ConfigureAwait(false);
    }
    
    protected override string GetCacheKey(
        TestSuitesRepositoryParameters? parameters,
        string itemName)
        => $"{CacheKeyPrefix}{parameters?.TestPlanName}_{itemName}";

    protected override string GetCacheKey(
        TestSuitesRepositoryParameters? parameters,
        TestSuite item)
            => GetCacheKey(parameters, item.Name);

    protected override async Task<IEnumerable<TestSuite>> LoadItemsAsync(
        TestSuitesRepositoryParameters? parameters,
        CancellationToken cancellationToken = default)
    {
        string testPlanName = parameters?.TestPlanName
            ?? throw new ArgumentNullException(
                nameof(parameters),
                "Test plan name must be provided as the first parameter.");
        
        TestPlan? testPlan = await _testPlansRepository
            .GetTestPlanByNameAsync(testPlanName, cancellationToken)
            .ConfigureAwait(false);

        if (testPlan == null)
        {
            throw new TestPlanNotFoundException(testPlanName);
        }
        
        return await _azureTestPlansGateway
            .GetTestSuitesAsync(_azureDevOpsOptions.ProjectName!, testPlan.Id, cancellationToken)
            .ConfigureAwait(false);
    }
}