namespace WhateverDotNet.Configuration;

/// <summary>
/// Standard set of test execution environments used by the WhateverDotNet test utilities.
/// </summary>
public enum TestEnvironment
{
    /// <summary>
    /// Development environment.
    /// </summary>
    Development,
    /// <summary>
    /// Quality Assurance environment.
    /// </summary>
    QA,
    /// <summary>
    /// INT environment (internal testing).
    /// </summary>
    INT,
    /// <summary>
    /// Staging environment.
    /// </summary>
    Staging,
    /// <summary>
    /// Production environment.
    /// </summary>
    Prod
}