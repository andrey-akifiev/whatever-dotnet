namespace WhateverDotNet.API.Abstractions.Logging;

/// <summary>
/// Abstraction for writing structured test-execution steps/results into a test runner output.
/// </summary>
public interface ITestExecutionLogger
{
    /// <summary>
    /// Logs the actual result observed during the test.
    /// </summary>
    /// <param name="message">Message describing the actual result.</param>
    public void LogActualResult(string message);

    /// <summary>
    /// Logs the expected result for the current verification.
    /// </summary>
    /// <param name="message">Message describing the expected result.</param>
    public void LogExpectedResult(string message);

    /// <summary>
    /// Logs a precondition that must be satisfied before continuing the test.
    /// </summary>
    /// <param name="message">Message describing the precondition.</param>
    public void LogPrecondition(string message);

    /// <summary>
    /// Logs a shared step that is reused across multiple tests or scenarios.
    /// </summary>
    /// <param name="message">Message describing the shared step.</param>
    public void LogSharedStep(string message);

    /// <summary>
    /// Logs a regular test step.
    /// </summary>
    /// <param name="message">Message describing the step.</param>
    public void LogStep(string message);
}