using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Feature : Element
{
    [JsonPropertyName("elements")]
    public Scenario[]? Scenarios { get; set; }
}