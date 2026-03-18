using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reporting.AzureDevOps.Repositories;
using WhateverDotNet.Reports.Abstractions;

namespace WhateverDotNet.Reporting.AzureDevOps;

public class AzureDevOpsSink(AzureDevOpsOptions options, TestCasesRepository testCasesRepository)
    : IAlmSink
{
    private readonly AzureDevOpsOptions _options = options
        ?? throw new ArgumentNullException(nameof(options));
    private readonly TestCasesRepository _testCasesRepository = testCasesRepository
        ?? throw new ArgumentNullException(nameof(testCasesRepository));
    
    public async Task SyncTestCasesAsync(
        WhateverTestCaseSpecification testCaseSpecification,
        IEnumerable<WhateverTestCase> testCases,
        CancellationToken cancellationToken = default)
    {
        string[] testCaseFields = TestCasesRepository
            .DefaultFields
            .Union(testCaseSpecification.StandardFields)
            .Union(testCaseSpecification.CustomFields)
            .Distinct()
            .ToArray();
        
        IEnumerable<int> existingTestCaseIds =
            (await _testCasesRepository
                .GetTestCasesAsync(
                    _options.ProjectName!,
                    _options.AreaPath,
                    testCaseFields,
                    cancellationToken)
                .ConfigureAwait(false))
            .Select(tc => tc.Id!.Value);

        IEnumerable<int> testCaseIdsToUpdate =
            existingTestCaseIds
                .Intersect(
                    // ReSharper disable once PossibleMultipleEnumeration
                    testCases
                        .Where(tc => tc.Id != null && int.TryParse(tc.Id, out _))
                        .Select(tc => int.Parse(tc.Id!)));

        foreach (var testCase in testCases)
        {
            if (testCase.Id is not null
                && int.TryParse(testCase.Id, out int testCaseId)
                // ReSharper disable once PossibleMultipleEnumeration
                && testCaseIdsToUpdate.Contains(testCaseId))
            {
                await _testCasesRepository
                    .UpdateTestCaseAsync(
                        _options.ProjectName!,
                        testCase,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await _testCasesRepository
                    .CreateTestCaseAsync(
                        _options.ProjectName!,
                        testCase,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    public Task SyncTestResultsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}