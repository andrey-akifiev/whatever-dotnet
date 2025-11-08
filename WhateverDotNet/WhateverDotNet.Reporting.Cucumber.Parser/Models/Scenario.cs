using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Scenario : Element
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("steps")]
    public Step[]? Steps { get; set; }
}