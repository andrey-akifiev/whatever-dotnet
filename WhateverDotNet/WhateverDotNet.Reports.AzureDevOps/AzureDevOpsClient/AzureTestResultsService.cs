using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

public class AzureTestResultsService(VssConnection vssConnection, ILoggerFactory? loggerFactory) : IDisposable
{
    private readonly ILogger<AzureTestPlansService>? _logger = loggerFactory?.CreateLogger<AzureTestPlansService>();
    private readonly TestResultsHttpClient _testResultsHttpClient = 
        vssConnection.GetClient<TestResultsHttpClient>()
        ?? throw new ArgumentNullException(nameof(vssConnection));

    public async Task<IEnumerable<TestCaseResult>> AddTestResultsAsync(
        string projectName,
        int testRunId,
        IEnumerable<TestCaseResult> testResults,
        CancellationToken cancellationToken = default)
    {
        TestCaseResult[] testCaseResults = testResults as TestCaseResult[] ?? testResults.ToArray();
        List<TestCaseResult> results = new(testCaseResults.Length);

        const int batchSize = 200;
        int currentTestResultIndex = 0;
        do
        {
            int currentBatchSize = Math.Min(batchSize, testCaseResults.Length - currentTestResultIndex);
            List<TestCaseResult>? response = await _testResultsHttpClient
                .AddTestResultsToTestRunAsync(
                    testCaseResults[currentTestResultIndex..(currentTestResultIndex + currentBatchSize)],
                    projectName,
                    testRunId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            currentTestResultIndex += currentBatchSize;
            results.AddRange(response);
        } while (currentTestResultIndex < testCaseResults.Length);

        return results;
    }

    public async Task<TestAttachmentReference> CreateTestResultAttachmentAsync(
        string projectName,
        int testRunId,
        int testCaseResultId,
        string attachmentName,
        Byte[] content,
        CancellationToken cancellationToken = default)
    {
        return await _testResultsHttpClient
            .CreateTestResultAttachmentAsync(
                new TestAttachmentRequestModel
                {
                    AttachmentType = "GeneralAttachment",
                    FileName = attachmentName,
                    Comment = "Attachment added by WhateverDotNet AzureDevOps Sync Tool",
                    Stream = await Task.Run(() => Convert.ToBase64String(content), cancellationToken),
                },
                projectName,
                testRunId,
                testCaseResultId,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    
    public async Task<TestRun> CreateTestRunAsync(
        string projectName,
        string testRunName,
        int testPlanId,
        string? releaseId = null,
        string? environmentId = null,
        CancellationToken cancellationToken = default)
    {
        return await _testResultsHttpClient
            .CreateTestRunAsync(
                new RunCreateModel(
                    name: testRunName,
                    isAutomated: true,
                    plan: new ShallowReference(id: testPlanId.ToString()),
                    releaseUri: 
                        string.IsNullOrWhiteSpace(releaseId)
                            ? null
                            : $"vstfs:///Release/Release/{releaseId}",
                    releaseEnvironmentUri:
                        string.IsNullOrWhiteSpace(environmentId)
                            ? null
                            : $"vstfs:///ReleaseManagement/Environment/{environmentId}"),
                projectName,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
    
    public void Dispose()
    {
        _testResultsHttpClient?.Dispose();
    }
}