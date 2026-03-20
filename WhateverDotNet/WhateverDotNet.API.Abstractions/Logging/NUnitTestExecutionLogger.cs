namespace WhateverDotNet.API.Abstractions.Logging;

/// <summary>
/// A basic <see cref="ITestExecutionLogger"/> implementation that writes messages to standard output.
/// Intended for use in NUnit-based test runs.
/// </summary>
public class NUnitTestExecutionLogger : ITestExecutionLogger
{
    private readonly List<string> _commonLogs = new ();

    private readonly TestExecutionLoggerOptions _options;

    /// <summary>
    /// Creates a logger that writes test preconditions/steps/results to standard output.
    /// </summary>
    /// <param name="options">Logger formatting options.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public NUnitTestExecutionLogger(TestExecutionLoggerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void AddCommonPrecondition(string message)
    {
        _commonLogs.Add(string.Format(_options.PreconditionPattern ?? "{0}", message));
    }

    /// <inheritdoc />
    public void AddCommonStep(string message)
    {
        _commonLogs.Add(string.Format(_options.StepPattern ?? "{0}", message));
    }

    /// <inheritdoc />
    public void LogActualResult(string message)
    {
        Console.WriteLine(_options.ActualResultPattern ?? "{0}", message);
    }

    /// <inheritdoc />
    public void LogCommonSteps()
    {
        foreach (string message in _commonLogs)
        {
            Console.WriteLine(message);
        }
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