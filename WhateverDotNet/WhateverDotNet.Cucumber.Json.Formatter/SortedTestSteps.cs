namespace WhateverDotNet.Cucumber.Json.Formatter;

internal class SortedTestSteps
{
    public List<InternalTestStep> BeforeHook { get; set; } = new();
    public List<InternalTestStep> Background { get; set; } = new();
    public List<InternalTestStep> Steps { get; set; } = new();
    public List<InternalTestStep> AfterHook { get; set; } = new();
}
