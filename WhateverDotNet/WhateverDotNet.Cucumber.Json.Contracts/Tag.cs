using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Contracts;

public class Tag
{
    [JsonPropertyName("line")]
    public uint Line { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}