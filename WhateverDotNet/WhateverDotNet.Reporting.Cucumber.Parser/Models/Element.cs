using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Element
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }
    
    [JsonPropertyName("line")]
    public int? Line { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("tags")]
    public Tag[]? Tags { get; set; }
    
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}