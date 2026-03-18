namespace WhateverDotNet.API.Abstractions.Logging;

public interface ITestExecutionLogger
{
    public void LogActualResult(string message);
    public void LogExpectedResult(string message);
    public void LogPrecondition(string message);
    public void LogSharedStep(string message);
    public void LogStep(string message);
}