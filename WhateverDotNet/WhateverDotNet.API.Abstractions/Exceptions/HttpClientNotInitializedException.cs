namespace WhateverDotNet.API.Abstractions.Exceptions;

/// <summary>
/// Thrown when an API call is attempted before the underlying <see cref="HttpClient"/> is initialized.
/// </summary>
public class HttpClientNotInitializedException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClientNotInitializedException"/> exception.
    /// </summary>
    public HttpClientNotInitializedException()
        : base("HttpClient instance is not initialized. Ensure that ConnectAs is executed before invoking API calls.")
    {
    }
}