namespace WhateverDotNet.API.Abstractions.Exceptions;

public class HttpClientNotInitializedException() : InvalidOperationException(
    "HttpClient instance is not initialized. Ensure that ConnectAs is executed before invoking API calls.");