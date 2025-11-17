using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Json.Contracts;

public class DocString
{
    [JsonPropertyName("content_type")]
    public string? ContentType { get; set; }

    [JsonPropertyName("line")]
    public uint Line { get; set; }

    [JsonPropertyName("value")]
    public string? Value { get; set; }
}