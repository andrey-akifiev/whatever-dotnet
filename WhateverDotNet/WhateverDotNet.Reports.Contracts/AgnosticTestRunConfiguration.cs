namespace WhateverDotNet.Reports.Contracts;

public class AgnosticTestRunConfiguration
{
    public string? Id { get; set; }
    
    public string? Name { get; set; }
    
    public IReadOnlyDictionary<string, string>? Values { get; set; }
}