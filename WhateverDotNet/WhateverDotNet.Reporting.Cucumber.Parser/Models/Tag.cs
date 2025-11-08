using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Tag
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("line")]
    public int? Line { get; set; }
}