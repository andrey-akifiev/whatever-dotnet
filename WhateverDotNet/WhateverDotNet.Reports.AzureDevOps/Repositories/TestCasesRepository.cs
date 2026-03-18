using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;

using WhateverDotNet.Reporting.AzureDevOps.Contracts;
using WhateverDotNet.Reporting.AzureDevOps.Extensions;
using WhateverDotNet.Reports.Abstractions;

// ReSharper disable AccessToStaticMemberViaDerivedType
namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class TestCasesRepository(WorkItemsRepository repository)
    : IDisposable
{
    public static readonly string[] DefaultFields =
        [
            TestCaseStandardFields.AreaPath,
            TestCaseStandardFields.Description,
            TestCaseStandardFields.Id,
            TestCaseStandardFields.Parameters,
            TestCaseStandardFields.State,
            TestCaseStandardFields.Steps,
            TestCaseStandardFields.Tags,
            TestCaseStandardFields.Title,
            TestCaseStandardFields.WorkItemType,
        ];
    
    private readonly WorkItemsRepository _workItemsRepository = repository
        ?? throw new ArgumentNullException(nameof(repository));

    // TODO: Should we really return something here? -- aa
    public Task<WorkItem> CreateTestCaseAsync(
        string projectName,
        WhateverTestCase testCase,
        CancellationToken cancellationToken = default)
        => _workItemsRepository
            .CreateWorkItemAsync(
                projectName,
                WorkItemTypes.TestCase,
                UnionTestCaseValues(testCase),
                cancellationToken);

    public void Dispose()
    {
        _workItemsRepository.Dispose();
    }
    
    public Task<IEnumerable<WorkItem>> GetTestCasesAsync(
        string projectName,
        string? areaPath = null,
        string[]? fields = null,
        CancellationToken cancellationToken = default)
        => _workItemsRepository
            .GetWorkItemsAsync(
                projectName,
                WorkItemTypes.TestCase,
                fields: fields ?? DefaultFields,
                areaPath,
                cancellationToken);
    
    public Task<WorkItem> UpdateTestCaseAsync(
        string projectName,
        WhateverTestCase testCase,
        CancellationToken cancellationToken = default)
        => _workItemsRepository
            .UpdateWorkItemAsync(
                projectName,
                WorkItemTypes.TestCase,
                int.Parse(testCase.Id ?? throw new InvalidOperationException()),
                UnionTestCaseValues(testCase),
                cancellationToken);

    private Dictionary<string, object?> UnionTestCaseValues(WhateverTestCase testCase)
    {
        var result = new Dictionary<string, object?>(
            testCase
                .ToFieldValueCollection()
                .ToDictionary(
                    fv => fv.ReferenceKey,
                    object? (fv) => fv.GetValue()));

        if (testCase.Attributes != null)
        {
            foreach (var attribute in testCase.Attributes)
            {
                result[attribute.Key] = attribute.Value;
            }
        }

        return result;
    }
}