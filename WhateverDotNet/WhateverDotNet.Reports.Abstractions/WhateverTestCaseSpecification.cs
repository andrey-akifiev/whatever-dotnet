namespace WhateverDotNet.Reports.Abstractions;

public class WhateverTestCaseSpecification
{
    public string[] StandardFields { get; init; } = Array.Empty<string>();
    public string[] CustomFields { get; init; } = Array.Empty<string>();
}