// ReSharper disable once CheckNamespace
namespace WhateverDotNet.Configuration;

/// <summary>
/// Options for configuring <c>EnvironmentVariablesProvider</c> lookups.
/// </summary>
public class EnvironmentOptions
{
    /// <summary>
    /// Optional prefix applied to environment variable names.
    /// </summary>
    /// <remarks>
    /// If <see cref="ProjectPrefix"/> is set to <c>MYAPP_</c> then a property named <c>Url</c>
    /// will be read from <c>MYAPP_URL</c> (and from <c>MYAPP_URL_<c>ENV</c></c> when environment-specific values exist).
    /// </remarks>
    public string? ProjectPrefix { get; set; }
}