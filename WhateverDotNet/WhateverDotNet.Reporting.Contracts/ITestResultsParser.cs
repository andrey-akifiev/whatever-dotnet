namespace WhateverDotNet.Reporting.Contracts;

public interface ITestResultsParser
{
    public Task<IEnumerable<AgnosticTestResult>> ParseAsync(CancellationToken cancellationToken);
}