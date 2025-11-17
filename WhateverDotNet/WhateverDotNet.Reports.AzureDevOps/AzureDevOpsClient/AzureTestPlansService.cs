using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

using WhateverDotNet.Reporting.AzureDevOps.Exceptions;

namespace WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

public class AzureTestPlansService(VssConnection vssConnection, ILoggerFactory? loggerFactory) : IDisposable
{
    private readonly ILogger<AzureTestPlansService>? _logger = loggerFactory?.CreateLogger<AzureTestPlansService>();
    private readonly TestPlanHttpClient? _testPlanHttpClient = vssConnection.GetClient<TestPlanHttpClient>();

    public async Task<TestConfiguration> CreateTestConfigurationAsync(
        string projectName,
        string configurationName,
        string? description = null,
        bool isDefault = false,
        IReadOnlyDictionary<string, string>? values = null,
        CancellationToken cancellationToken = default)
    {
        return await _testPlanHttpClient!
            .CreateTestConfigurationAsync(
                new TestConfigurationCreateUpdateParameters
                {
                    Name = configurationName,
                    Description = description,
                    IsDefault = isDefault,
                    State = Microsoft.TeamFoundation.TestManagement.WebApi.TestConfigurationState.Active,
                    Values = values
                        ?.Select(v =>
                            new Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair(v.Key, v.Value))
                        .ToList(),
                },
                projectName,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    
    public async Task<TestVariable> CreateTestVariableAsync(
        string projectName,
        string variableName,
        string? description = null,
        IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        return await _testPlanHttpClient!   
            .CreateTestVariableAsync(
                new TestVariableCreateUpdateParameters
                {
                    Name = variableName,
                    Description = description,
                    Values = values?.ToList(),
                },
                projectName,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<TestConfiguration>> GetTestConfigurationsAsync(
        string projectName,
        CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;
        var result = new List<TestConfiguration>();

        do
        {
            PagedList<TestConfiguration> response = await _testPlanHttpClient!
                .GetTestConfigurationsAsync(
                    project: projectName,
                    continuationToken: continuationToken,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            result.AddRange(response);
            continuationToken = response.ContinuationToken;
        }
        while (!string.IsNullOrEmpty(continuationToken));

        return result;
    }

    public async Task<IEnumerable<TestVariable>> GetTestVariablesAsync(
        string projectName,
        CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;
        var result = new List<TestVariable>();

        do
        {
            PagedList<TestVariable> response = await _testPlanHttpClient!
                .GetTestVariablesAsync(
                    project: projectName,
                    continuationToken: continuationToken,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            result.AddRange(response);
            continuationToken = response.ContinuationToken;
        }
        while (!string.IsNullOrEmpty(continuationToken));

        return result;
    }
    
    public async Task<IEnumerable<TestCase>> GetTestCasesByTestSuiteAsync(
        string projectName,
        int testPlanId,
        int testSuiteId,
        CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;
        var result = new List<TestCase>();

        do
        {
            PagedList<TestCase> response = await _testPlanHttpClient!
                .GetTestCaseListAsync(
                    project: projectName,
                    planId: testPlanId,
                    suiteId: testSuiteId,
                    returnIdentityRef: true,
                    continuationToken: continuationToken,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            result.AddRange(response);
            continuationToken = response.ContinuationToken;
        }
        while (!string.IsNullOrEmpty(continuationToken));

        return result;
    }

    public async Task<TestPlan> GetTestPlanByNameAsync(
        string projectName,
        string testPlanName,
        CancellationToken cancellationToken = default)
    {
        PagedList<TestPlan>? testPlans = await _testPlanHttpClient!
            .GetTestPlansAsync(
                project: projectName,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        TestPlan? result = testPlans.FirstOrDefault(tp => tp.Name.Equals(testPlanName, StringComparison.OrdinalIgnoreCase));

        if (result == null)
        {
            var ex = new TestPlanNotFoundException(projectName, testPlanName);
            _logger?.LogError(ex, ex.Message);
            throw ex;
        }
        
        return result;
    }
    
    public async Task<IEnumerable<TestPlan>> GetTestPlansAsync(
        string projectName,
        CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;
        var result = new List<TestPlan>();

        do
        {
            PagedList<TestPlan> response = await _testPlanHttpClient!
                .GetTestPlansAsync(
                    project: projectName,
                    continuationToken: continuationToken,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            result.AddRange(response);
            continuationToken = response.ContinuationToken;
        }
        while (!string.IsNullOrEmpty(continuationToken));

        return result;
    }
    
    public async Task<IEnumerable<TestPoint>> GetTestPointsAsync(
        string projectName,
        int testPlanId,
        int testSuiteId,
        string testCaseId,
        CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;
        var result = new List<TestPoint>();

        do
        {
            PagedList<TestPoint> response = await _testPlanHttpClient!
                .GetPointsListAsync(
                    project: projectName,
                    planId: testPlanId,
                    suiteId: testSuiteId,
                    testCaseId: testCaseId,
                    continuationToken: continuationToken,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            result.AddRange(response);
            continuationToken = response.ContinuationToken;
        }
        while (!string.IsNullOrEmpty(continuationToken));

        return result;
    }

    public async Task<TestSuite> GetTestSuiteByNameAsync(
        string projectName,
        int testPlanId,
        string testSuiteName,
        CancellationToken cancellationToken = default)
    {
        var result = (await GetTestSuitesAsync(projectName, testPlanId, cancellationToken)
            .ConfigureAwait(false))
            .FirstOrDefault(ts => ts.Name.Equals(testSuiteName, StringComparison.OrdinalIgnoreCase));

        if (result == null)
        {
            var ex = new TestSuiteNotFoundException(projectName, testPlanId, testSuiteName);
            _logger?.LogError(ex, ex.Message);
            throw ex;
        }
        
        return result;
    }

    public async Task<IEnumerable<TestSuite>> GetTestSuitesAsync(
        string projectName,
        int testPlanId,
        CancellationToken cancellationToken = default)
    {
        string? continuationToken = null;
        var result = new List<TestSuite>();

        do
        {
            PagedList<TestSuite> response = await _testPlanHttpClient!
                .GetTestSuitesForPlanAsync(
                    project: projectName,
                    planId: testPlanId,
                    continuationToken: continuationToken,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            result.AddRange(response);
            continuationToken = response.ContinuationToken;
        }
        while (!string.IsNullOrEmpty(continuationToken));

        return result;
    }

    public void Dispose()
    {
        _testPlanHttpClient?.Dispose();
    }

    public async Task<TestVariable> UpdateTestVariableAsync(
        string projectName,
        int testVariableId,
        string variableName,
        string? description = null,
        IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        return await _testPlanHttpClient!   
            .UpdateTestVariableAsync(
                new TestVariableCreateUpdateParameters
                {
                    Name = variableName,
                    Description = description,
                    Values = values?.ToList(),
                },
                projectName,
                testVariableId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}