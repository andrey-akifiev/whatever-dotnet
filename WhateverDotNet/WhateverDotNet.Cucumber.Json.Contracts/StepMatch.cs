using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Contracts;

public class StepMatch
{
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}