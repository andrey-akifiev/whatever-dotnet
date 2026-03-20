namespace WhateverDotNet.API.Abstractions.Logging;

/// <summary>
/// Abstraction for writing structured test-execution steps/results into a test runner output.
/// </summary>
public interface ITestExecutionLogger
{
    /// <summary>
    /// Adds a common precondition message that will be included in the test output.
    /// Used for test framework what does not support test execution logs shared between tests, such as NUnit.
    /// </summary>
    /// <param name="message">Message describing the common precondition.</param>
    public void AddCommonPrecondition(string message);
    
    /// <summary>
    /// Adds a common step message that will be included in the test output.
    /// Used for test framework what does not support test execution logs shared between tests, such as NUnit.
    /// </summary>
    /// <param name="message">Message describing the common step.</param>
    public void AddCommonStep(string message);
    
    /// <summary>
    /// Logs the actual result observed during the test.
    /// </summary>
    /// <param name="message">Message describing the actual result.</param>
    public void LogActualResult(string message);

    /// <summary>
    /// Logs common preconditions/steps that were previously added via
    /// <see cref="AddCommonPrecondition"/> and <see cref="AddCommonStep"/>.
    /// Used for test framework what does not support test execution logs shared between tests, such as NUnit.
    /// </summary>
    public void LogCommonSteps();
    
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