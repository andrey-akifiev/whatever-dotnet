namespace WhateverDotNet.Reports.Abstractions;

public class WhateverTestCase
{
    public Dictionary<string, object>? Attributes { get; set; }
    
    public string? Id { get; set; }
    
    public string? Title { get; init; }
    
    public IEnumerable<WhateverTestStep>? TestSteps { get; init; }
}