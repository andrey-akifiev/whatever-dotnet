using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Contracts;

public class Feature
{
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("elements")]
    public List<Scenario> Elements { get; set; }

    [JsonPropertyName("id")]
    public string ID { get; set; }

    [JsonPropertyName("keyword")]
    public string Keyword { get; set; }

    [JsonPropertyName("line")]
    public uint Line { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("uri")]
    public string URI { get; set; }

    [JsonPropertyName("tags")]
    public List<Tag>? Tags { get; set; }
}