using System.Text.Json.Serialization;

namespace WhateverDotNet.Reporting.Cucumber.Parser.Models;

public class Match
{
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}