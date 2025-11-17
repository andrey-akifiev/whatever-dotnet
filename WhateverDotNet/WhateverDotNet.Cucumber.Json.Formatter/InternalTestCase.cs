using Io.Cucumber.Messages.Types;

namespace WhateverDotNet.Reporting.JsonFormatter;
    
internal class InternalTestCase
{
    public string FeatureName { get; set; }
    public Scenario Scenario { get; set; }
    public Pickle Pickle { get; set; }
    public TestCase TestCase { get; set; }
    public List<InternalTestStep> Steps { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();

    public void AppendStep(InternalTestStep step)
    {
        Steps.Add(step);
    }

    public SortedTestSteps SortedSteps()
    {
        var sorted = new SortedTestSteps();
        List<InternalTestStep> current = sorted.BeforeHook;

        foreach (var step in Steps)
        {
            if (ReferenceEquals(current, sorted.BeforeHook) && step.Hook == null)
            {
                current = sorted.Background;
            }
            if (ReferenceEquals(current, sorted.Background) && step.Background == null)
            {
                current = sorted.Steps;
            }
            if (ReferenceEquals(current, sorted.Steps) && step.Hook != null)
            {
                current = sorted.AfterHook;
            }
            current.Add(step);
        }

        return sorted;
    }
}