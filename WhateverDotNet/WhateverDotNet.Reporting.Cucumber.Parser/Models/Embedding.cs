using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Embedding
{
    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }
    
    [JsonPropertyName("data")]
    public string? Data { get; set; }
}