using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Result
{
    [JsonPropertyName("duration")]
    public long? Duration { get; set; }
    
    [JsonPropertyName("status")]
    public string? Status { get; set; }
    
    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}