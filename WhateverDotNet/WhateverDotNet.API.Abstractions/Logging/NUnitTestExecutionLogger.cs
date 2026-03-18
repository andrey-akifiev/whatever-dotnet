namespace WhateverDotNet.API.Abstractions.Logging;

public class NUnitTestExecutionLogger(TestExecutionLoggerOptions options) : ITestExecutionLogger
{
    private readonly TestExecutionLoggerOptions _options = options
        ?? throw new ArgumentNullException(nameof(options));

    public void LogActualResult(string message)
    {
        Console.WriteLine(_options.ActualResultPattern ?? "{0}", message);
    }

    public void LogExpectedResult(string message)
    {
        Console.WriteLine(_options.ExpectedResultPattern ?? "{0}", message);
    }

    public void LogPrecondition(string message)
    {
        Console.WriteLine(_options.PreconditionPattern ?? "{0}", message);
    }

    public void LogSharedStep(string message)
    {
        throw new NotImplementedException();
    }

    public void LogStep(string message)
    {
        Console.WriteLine(_options.StepPattern ?? "{0}", message);
    }
}