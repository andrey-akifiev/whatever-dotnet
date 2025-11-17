using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Json.Contracts;

public class Embedding
{
    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("mime_type")]
    public string? MimeType { get; set; }
}