using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using WhateverDotNet.API.Abstractions.Exceptions;
using WhateverDotNet.API.Abstractions.Logging;

namespace WhateverDotNet.API.Abstractions;

/// <summary>
/// Base class for JSON-based REST API clients.
/// Provides helper methods for sending JSON requests, logging steps, and deserializing JSON responses.
/// </summary>
/// <param name="options">Client configuration options (base URL, timeout).</param>
/// <param name="logFormatter">Formatter used to produce readable API call messages.</param>
/// <param name="apiLogger">Optional test-execution logger for recording steps.</param>
/// <param name="loggerFactory">Optional logger factory for structured tracing.</param>
public abstract class BaseJsonRestClient(
    BaseJsonRestClientOptions options,
    ILogMessageFormatter logFormatter,
    ITestExecutionLogger? apiLogger,
    ILoggerFactory? loggerFactory = null)
    : IDisposable
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
    };
    
    private readonly ILogMessageFormatter _formatter = logFormatter
        ?? throw new ArgumentNullException(nameof(logFormatter));
    
    /// <summary>
    /// Optional logger for recording API call steps in test execution logs.
    /// If not provided, API calls will not be logged in test reports, but structured logging via <see cref="_logger"/> may still occur if a logger factory is given.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected readonly ITestExecutionLogger? _apiLogger = apiLogger 
        ?? throw new ArgumentNullException(nameof(apiLogger));
    
    /// <summary>
    /// Optional logger for recording API call details in structured logs (e.g. for debugging or telemetry).
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected readonly ILogger<BaseJsonRestClient>? _logger =
        loggerFactory?.CreateLogger<BaseJsonRestClient>();
    
    /// <summary>
    /// Client configuration options (base URL, timeout).
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected readonly BaseJsonRestClientOptions _options = options
        ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Decorated regular <see cref="HttpClient"/>.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected HttpClient? _httpClient;

    /// <summary>
    /// Disposes the underlying <see cref="HttpClient"/> (if created).
    /// </summary>
    public virtual void Dispose()
    {
        _httpClient?.Dispose();
    }

    /// <summary>
    /// Attempts to deserialize the response content as JSON into <typeparamref name="TResponse"/>.
    /// </summary>
    /// <param name="responseMessage">HTTP response message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <typeparam name="TResponse">Expected response DTO type.</typeparam>
    /// <returns>
    /// A tuple containing the deserialized response (or <see langword="default"/> when content is empty or invalid)
    /// and the original <paramref name="responseMessage"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="responseMessage"/> or its content is <see langword="null"/>.</exception>
    protected static async Task<(TResponse? Response, HttpResponseMessage Message)> DeserializeResponseAsJsonAsync<TResponse>(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default)
    {
        if (responseMessage?.Content is null)
        {
            throw new ArgumentNullException(nameof(responseMessage));
        }

        string responseContent = await responseMessage
            .Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);
        
        if (string.IsNullOrWhiteSpace(responseContent))
        {
            return (default, responseMessage);
        }

        try
        {
            TResponse? deserializedResponse = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            return (deserializedResponse, responseMessage);
        }
        catch (Exception)
        {
            return (default, responseMessage);
        }
    }

    /// <summary>
    /// Creates and configures an <see cref="HttpClient"/> for this API client.
    /// </summary>
    /// <param name="authorizationToken">Authorization token/header value to send with requests.</param>
    /// <param name="additionalHeaders">Optional additional headers to attach to all requests.</param>
    /// <returns>A configured <see cref="HttpClient"/> instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="authorizationToken"/> is blank, or when <see cref="BaseJsonRestClientOptions.BaseUrl"/> is not set.
    /// </exception>
    protected virtual HttpClient CreateClient(
        string authorizationToken,
        IReadOnlyDictionary<string, string>? additionalHeaders = null)
    {
        if (string.IsNullOrWhiteSpace(authorizationToken))
        {
            throw new ArgumentNullException(nameof(authorizationToken));
        }
        
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            throw new ArgumentNullException(nameof(_options.BaseUrl));
        }

        HttpClient httpClient = new ()
        {
            BaseAddress = new Uri(_options.BaseUrl),
            Timeout = _options.Timeout,
        };

        if (additionalHeaders != null)
        {
            foreach ((string headerName, string headerValue) in additionalHeaders)
            {
                httpClient
                    .DefaultRequestHeaders
                    .TryAddWithoutValidation(headerName, headerValue);
            }
        }
        
        httpClient
            .DefaultRequestHeaders
            .TryAddWithoutValidation(HeaderNames.Accept, MediaTypeNames.Application.Json);
        httpClient
            .DefaultRequestHeaders
            .Add(HeaderNames.Authorization, authorizationToken);
        
        return httpClient;
    }

    /// <summary>
    /// Creates request content for a JSON payload.
    /// </summary>
    /// <param name="payload">Serialized JSON payload.</param>
    /// <returns>A <see cref="StringContent"/> with JSON media type and UTF-8 encoding.</returns>
    protected virtual StringContent CreateJsonContent(string payload)
        => new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);

    /// <summary>
    /// Executes an API call and asserts that its result matches the expected HTTP status code.
    /// </summary>
    /// <param name="function">Function performing the API call and returning deserialized response and raw message.</param>
    /// <param name="expectedStatusCode">Expected HTTP status code.</param>
    /// <typeparam name="TResponse">Response DTO type.</typeparam>
    /// <returns>The deserialized response.</returns>
    protected abstract Task<TResponse> ExecuteAndAssertAsync<TResponse>(
        Func<Task<(TResponse? Response, HttpResponseMessage Message)>> function,
        HttpStatusCode expectedStatusCode);

    /// <summary>
    /// Sends an HTTP DELETE request and attempts to deserialize the response as JSON.
    /// </summary>
    /// <param name="endpointPath">Endpoint path relative to <see cref="BaseJsonRestClientOptions.BaseUrl"/>.</param>
    /// <param name="requestBody">Optional request body object to serialize as JSON.</param>
    /// <param name="additionalHeaders">Optional request headers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="callerClientMethodName">Caller method name used for logging.</param>
    /// <typeparam name="TResponse">Response DTO type.</typeparam>
    /// <returns>A tuple with the deserialized response (if any) and the raw HTTP message.</returns>
    protected virtual async Task<(TResponse? Response, HttpResponseMessage Message)> TryDeleteAsJsonAsync<TResponse>(
        string endpointPath,
        object? requestBody = null,
        IReadOnlyDictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string? callerClientMethodName = null)
    {
        HttpResponseMessage responseMessage =
            await SendAsJsonAsync(
                    HttpMethod.Delete,
                    endpointPath,
                    requestBody,
                    additionalHeaders,
                    callerClientMethodName,
                    cancellationToken)
                .ConfigureAwait(false);
        return await DeserializeResponseAsJsonAsync<TResponse>(
                responseMessage,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP GET request and attempts to deserialize the response as JSON.
    /// </summary>
    /// <param name="endpointPath">Endpoint path relative to <see cref="BaseJsonRestClientOptions.BaseUrl"/>.</param>
    /// <param name="additionalHeaders">Optional request headers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="callerClientMethodName">Caller method name used for logging.</param>
    /// <typeparam name="TResponse">Response DTO type.</typeparam>
    /// <returns>A tuple with the deserialized response (if any) and the raw HTTP message.</returns>
    protected virtual async Task<(TResponse? Response, HttpResponseMessage Message)> TryGetAsJsonAsync<TResponse>(
        string endpointPath,
        IReadOnlyDictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string? callerClientMethodName = null)
    {
        HttpResponseMessage responseMessage =
            await SendAsJsonAsync(
                    HttpMethod.Get,
                    endpointPath,
                    null,
                    additionalHeaders,
                    callerClientMethodName,
                    cancellationToken)
                .ConfigureAwait(false);
        return await DeserializeResponseAsJsonAsync<TResponse>(
                responseMessage,
                cancellationToken)
            .ConfigureAwait(false);
    }
    
    /// <summary>
    /// Sends an HTTP POST request and attempts to deserialize the response as JSON.
    /// </summary>
    /// <param name="endpointPath">Endpoint path relative to <see cref="BaseJsonRestClientOptions.BaseUrl"/>.</param>
    /// <param name="requestBody">Request body object to serialize as JSON.</param>
    /// <param name="additionalHeaders">Optional request headers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="callerClientMethodName">Caller method name used for logging.</param>
    /// <typeparam name="TResponse">Response DTO type.</typeparam>
    /// <returns>A tuple with the deserialized response (if any) and the raw HTTP message.</returns>
    protected virtual async Task<(TResponse? Response, HttpResponseMessage Message)> TryPostAsJsonAsync<TResponse>(
        string endpointPath,
        object? requestBody,
        IReadOnlyDictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string? callerClientMethodName = null)
    {
        HttpResponseMessage responseMessage =
            await SendAsJsonAsync(
                    HttpMethod.Post,
                    endpointPath,
                    requestBody,
                    additionalHeaders,
                    callerClientMethodName,
                    cancellationToken)
                .ConfigureAwait(false);
        return await DeserializeResponseAsJsonAsync<TResponse>(
                responseMessage,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP PUT request and attempts to deserialize the response as JSON.
    /// </summary>
    /// <param name="endpointPath">Endpoint path relative to <see cref="BaseJsonRestClientOptions.BaseUrl"/>.</param>
    /// <param name="requestBody">Request body object to serialize as JSON.</param>
    /// <param name="additionalHeaders">Optional request headers.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="callerClientMethodName">Caller method name used for logging.</param>
    /// <typeparam name="TResponse">Response DTO type.</typeparam>
    /// <returns>A tuple with the deserialized response (if any) and the raw HTTP message.</returns>
    protected virtual async Task<(TResponse? Response, HttpResponseMessage Message)> TryPutAsJsonAsync<TResponse>(
        string endpointPath,
        object? requestBody,
        IReadOnlyDictionary<string, string>? additionalHeaders = null,
        CancellationToken cancellationToken = default,
        [CallerMemberName] string? callerClientMethodName = null)
    {
        HttpResponseMessage responseMessage =
            await SendAsJsonAsync(
                HttpMethod.Put,
                endpointPath,
                requestBody,
                additionalHeaders,
                callerClientMethodName,
                cancellationToken)
            .ConfigureAwait(false);
        return await DeserializeResponseAsJsonAsync<TResponse>(
                responseMessage,
                cancellationToken)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Sends an HTTP request with an optional JSON body and logs it using configured loggers.
    /// </summary>
    /// <param name="method">HTTP method.</param>
    /// <param name="endpointPath">Endpoint path relative to <see cref="BaseJsonRestClientOptions.BaseUrl"/>.</param>
    /// <param name="requestBody">Optional request body object to serialize as JSON.</param>
    /// <param name="additionalHeaders">Optional request headers.</param>
    /// <param name="callerClientMethodName">Caller method name used for logging.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The raw <see cref="HttpResponseMessage"/>.</returns>
    /// <exception cref="HttpClientNotInitializedException">
    /// Thrown when <see cref="_httpClient"/> is not initialized.
    /// </exception>
    protected virtual Task<HttpResponseMessage> SendAsJsonAsync(
        HttpMethod method,
        string endpointPath,
        object? requestBody,
        IReadOnlyDictionary<string, string>? additionalHeaders = null,
        string? callerClientMethodName = null,
        CancellationToken cancellationToken = default)
    {
        if (_httpClient == null)
        {
            throw new HttpClientNotInitializedException();
        }
        
        using var request = new HttpRequestMessage(method, endpointPath);

        if (additionalHeaders is not null && additionalHeaders.Count > 0)
        {
            foreach ((string headerName, string headerValue) in additionalHeaders)
            {
                request.Headers.TryAddWithoutValidation(headerName, headerValue);
            }
        }

        string? serializedPayload = SerializeJsonPayload(requestBody);
        
        if (serializedPayload is not null)
        {
            request.Content = CreateJsonContent(serializedPayload);
        }
        
        _logger?.LogTrace(
            "Sending {HttpMethod} request to endpoint {EndpointPath} with body: {RequestBody}",
            method,
            endpointPath,
            requestBody);
        _apiLogger?.LogStep(
            _formatter.FormatLogMessage(
                callerClientMethodName,
                method.ToString(),
                endpointPath,
                serializedPayload));

        return _httpClient.SendAsync(request, cancellationToken);
    }
    
    /// <summary>
    /// Serializes an object into JSON using the default serializer settings of this base client.
    /// </summary>
    /// <param name="payload">Payload object.</param>
    /// <returns>Serialized JSON, or <see langword="null"/> when <paramref name="payload"/> is <see langword="null"/>.</returns>
    protected virtual string? SerializeJsonPayload(object? payload)
        => payload is null
            ? null
            : JsonSerializer.Serialize(payload, JsonOptions);
}