namespace WhateverDotNet.API.Abstractions.Exceptions;

/// <summary>
/// Thrown when an API call is attempted before the underlying <see cref="HttpClient"/> is initialized.
/// </summary>
public class HttpClientNotInitializedException() : InvalidOperationException(
    "HttpClient instance is not initialized. Ensure that ConnectAs is executed before invoking API calls.");