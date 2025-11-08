using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Step
{
    [JsonPropertyName("embeddings")]
    public Embedding[]? Embeddings { get; set; }
    
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }
    
    [JsonPropertyName("line")]
    public int? Line { get; set; }
    
    [JsonPropertyName("match")]
    public Match? Match { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("result")]
    public Result? Result { get; set; }
}