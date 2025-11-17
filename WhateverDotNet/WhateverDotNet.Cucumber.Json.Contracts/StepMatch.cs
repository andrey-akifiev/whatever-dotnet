using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Json.Contracts;

public class StepMatch
{
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}