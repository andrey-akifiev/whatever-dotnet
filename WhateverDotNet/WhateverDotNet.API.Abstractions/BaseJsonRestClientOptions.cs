namespace WhateverDotNet.API.Abstractions;

/// <summary>
/// Configuration options for <see cref="BaseJsonRestClient"/> implementations.
/// </summary>
public class BaseJsonRestClientOptions
{
    /// <summary>
    /// Base URL of the API (for example, <c>https://api.example.com/</c>).
    /// </summary>
    public string? BaseUrl { get; set; }
    
    /// <summary>
    /// HTTP request timeout used by the underlying <see cref="HttpClient"/>.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
}