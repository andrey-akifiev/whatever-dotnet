using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Contracts;

public class DatatableRow
{
    [JsonPropertyName("cells")]
    public List<string>? Cells { get; set; }
}