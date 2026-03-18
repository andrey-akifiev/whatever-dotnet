namespace WhateverDotNet.Reports.Abstractions;

public interface IAlmSink
{
    public Task SyncTestCasesAsync(
        WhateverTestCaseSpecification testCaseSpecification,
        IEnumerable<WhateverTestCase> testCases,
        CancellationToken cancellationToken = default);
    
    public Task SyncTestResultsAsync(
        CancellationToken cancellationToken = default);
}