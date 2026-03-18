namespace WhateverDotNet.Reports.Abstractions;

public class AgnosticTestRunConfiguration
{
    public string? Id { get; set; }
    
    public string? Name { get; set; }
    
    public IReadOnlyDictionary<string, string>? Values { get; set; }
}