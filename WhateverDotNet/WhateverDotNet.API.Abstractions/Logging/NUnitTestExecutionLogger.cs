namespace WhateverDotNet.API.Abstractions.Logging;

/// <summary>
/// A basic <see cref="ITestExecutionLogger"/> implementation that writes messages to standard output.
/// Intended for use in NUnit-based test runs.
/// </summary>
public class NUnitTestExecutionLogger(TestExecutionLoggerOptions options) : ITestExecutionLogger
{
    private readonly TestExecutionLoggerOptions _options = options
        ?? throw new ArgumentNullException(nameof(options));

    /// <inheritdoc />
    public void LogActualResult(string message)
    {
        Console.WriteLine(_options.ActualResultPattern ?? "{0}", message);
    }

    /// <inheritdoc />
    public void LogExpectedResult(string message)
    {
        Console.WriteLine(_options.ExpectedResultPattern ?? "{0}", message);
    }

    /// <inheritdoc />
    public void LogPrecondition(string message)
    {
        Console.WriteLine(_options.PreconditionPattern ?? "{0}", message);
    }

    /// <inheritdoc />
    /// <exception cref="NotImplementedException">
    /// Thrown because shared step logging is not implemented for this logger yet.
    /// </exception>
    public void LogSharedStep(string message)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void LogStep(string message)
    {
        Console.WriteLine(_options.StepPattern ?? "{0}", message);
    }
}