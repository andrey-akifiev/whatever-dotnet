using System.Text.Json.Serialization;

namespace WhateverDotNet.Cucumber.Json.Contracts;

public class StepResult
{
    [JsonPropertyName("duration")]
    public ulong? Duration { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}