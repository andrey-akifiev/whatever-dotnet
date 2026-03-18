namespace WhateverDotNet.API.Abstractions.Logging;

/// <summary>
/// Format patterns used by <see cref="ITestExecutionLogger"/> implementations.
/// </summary>
public class TestExecutionLoggerOptions
{
    /// <summary>
    /// Format string used for <see cref="ITestExecutionLogger.LogActualResult"/>.
    /// </summary>
    public string? ActualResultPattern { get; set; }

    /// <summary>
    /// Format string used for <see cref="ITestExecutionLogger.LogExpectedResult"/>.
    /// </summary>
    public string? ExpectedResultPattern { get; set; }

    /// <summary>
    /// Format string used for <see cref="ITestExecutionLogger.LogPrecondition"/>.
    /// </summary>
    public string? PreconditionPattern { get; set; }

    /// <summary>
    /// Format string used for <see cref="ITestExecutionLogger.LogSharedStep"/>.
    /// </summary>
    public string? SharedStepPattern { get; set; }

    /// <summary>
    /// Format string used for <see cref="ITestExecutionLogger.LogStep"/>.
    /// </summary>
    public string? StepPattern { get; set; }
}