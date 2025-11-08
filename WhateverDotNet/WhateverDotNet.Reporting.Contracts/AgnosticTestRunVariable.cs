namespace WhateverDotNet.Reporting.Contracts;

public class AgnosticTestRunVariable
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public IEnumerable<string>? Values { get; set; }
}