using Io.Cucumber.Messages.Types;

namespace WhateverDotNet.Cucumber.Json.Formatter;

internal class InternalTestStep
{
    public string TestCaseID { get; set; }
    public Hook? Hook { get; set; }
    public Pickle? Pickle { get; set; }
    public PickleStep? PickleStep { get; set; }
    public Step? Step { get; set; }
    public List<StepDefinition>? StepDefinitions { get; set; }
    public TestStepResult? Result { get; set; }
    public Background? Background { get; set; }
    public List<Attachment>? Attachments { get; set; }
    public TableRow? ExampleRow { get; set; }
}
