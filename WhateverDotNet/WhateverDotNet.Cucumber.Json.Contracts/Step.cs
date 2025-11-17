using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Json.Contracts;

public class Step
{
    [JsonPropertyName("keyword")]
    public string? Keyword { get; set; }

    [JsonPropertyName("line")]
    public uint? Line { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("result")]
    public StepResult? Result { get; set; }

    [JsonPropertyName("match")]
    public StepMatch? Match { get; set; }

    [JsonPropertyName("doc_string")]
    public DocString? DocString { get; set; }

    [JsonPropertyName("rows")]
    public List<DatatableRow>? Rows { get; set; }

    [JsonPropertyName("embeddings")]
    public List<Embedding>? Embeddings { get; set; }

    [JsonPropertyName("output")]
    public List<string>? Output { get; set; }
}