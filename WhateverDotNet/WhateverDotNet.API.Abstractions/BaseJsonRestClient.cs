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

    // ReSharper disable once InconsistentNaming
    protected readonly ITestExecutionLogger? _apiLogger = apiLogger 
        ?? throw new ArgumentNullException(nameof(apiLogger));
    
    // ReSharper disable once InconsistentNaming
    protected readonly ILogger<BaseJsonRestClient>? _logger =
        loggerFactory?.CreateLogger<BaseJsonRestClient>();
    
    // ReSharper disable once InconsistentNaming
    protected readonly BaseJsonRestClientOptions _options = options
        ?? throw new ArgumentNullException(nameof(options));

    // ReSharper disable once InconsistentNaming
    protected HttpClient? _httpClient;

    public virtual void Dispose()
    {
        _httpClient?.Dispose();
    }

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

    protected virtual StringContent CreateJsonContent(string payload)
        => new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);

    protected abstract Task<TResponse> ExecuteAndAssertAsync<TResponse>(
        Func<Task<(TResponse? Response, HttpResponseMessage Message)>> function,
        HttpStatusCode expectedStatusCode);

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
    
    protected virtual string? SerializeJsonPayload(object? payload)
        => payload is null
            ? null
            : JsonSerializer.Serialize(payload, JsonOptions);
}