namespace WhateverDotNet.API.Abstractions.Logging;

public class TestExecutionLoggerOptions
{
    public string? ActualResultPattern { get; set; }
    public string? ExpectedResultPattern { get; set; }
    public string? PreconditionPattern { get; set; }
    public string? SharedStepPattern { get; set; }
    public string? StepPattern { get; set; }
}