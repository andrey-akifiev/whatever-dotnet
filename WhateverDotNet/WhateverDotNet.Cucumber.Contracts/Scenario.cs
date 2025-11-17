using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Contracts;

public class Scenario
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("id")]
    public string? ID { get; set; }

    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }

    [JsonPropertyName("line")]
    public uint Line { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("before")]
    public List<Step>? Before { get; set; }

    [JsonPropertyName("steps")]
    public List<Step> Steps { get; set; }

    [JsonPropertyName("after")]
    public List<Step>? After { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("tags")]
    public List<Tag>? Tags { get; set; }
}