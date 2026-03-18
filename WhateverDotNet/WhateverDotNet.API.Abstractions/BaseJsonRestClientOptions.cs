namespace WhateverDotNet.API.Abstractions;

public class BaseJsonRestClientOptions
{
    public string? BaseUrl { get; set; }
    
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
}