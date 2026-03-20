# WhateverDotNet.API.Abstractions

Abstractions and helpers for building **JSON-based REST API clients**, including:

- `BaseJsonRestClient`: reusable base class for sending JSON requests and deserializing JSON responses
- DI helpers to register logging utilities
- Test-execution logging abstractions (useful for NUnit / BDD-style step output)

## Install

```bash
dotnet add package WhateverDotNet.API.Abstractions --prerelease
```

## What’s inside

- **Base client**: `WhateverDotNet.API.Abstractions.BaseJsonRestClient`
  - Sends requests via an initialized `HttpClient`
  - Serializes request bodies to JSON and deserializes responses from JSON
  - Logs outgoing calls (trace logger + optional test-step logger)
- **Options**: `BaseJsonRestClientOptions` (`BaseUrl`, `Timeout`)
- **Logging**:
  - `ITestExecutionLogger` (test output abstraction)
  - `NUnitTestExecutionLogger` (writes to `Console`)
  - `ILogMessageFormatter` / `ApiLogMessageFormatter` (human-readable step messages)
- **DI**: `ServiceCollectionExtensions`
  - `AddApiLogMessageFormatter()`
  - `AddNUnitTestExecutionLogger(options)`

## Basic usage

Create your API client by inheriting from `BaseJsonRestClient` and implementing `ExecuteAndAssertAsync`.

```csharp
using System.Net;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using WhateverDotNet.API.Abstractions;
using WhateverDotNet.API.Abstractions.Logging;

public sealed class MyApiClient : IDisposable
{
    private BaseJsonRestClient _client;

    public MyApiClient(
        BaseJsonRestClientOptions options,
        ILogMessageFormatter formatter,
        ITestExecutionLogger? apiLogger,
        ILoggerFactory? loggerFactory = null)
    {
        _client = new BaseJsonRestClient(options, logFormatter, testExecutionLogger, loggerFactory);
    }

    public void ConnectAs(string bearerToken)
    {
        // The base class throws if BaseUrl/token are missing.
        CreateClient($"Bearer {bearerToken}");
    }

    public void Dispose()
    {
        _client?.Dispose();
    }

    public Task<MyDto> GetSomethingAsync(CancellationToken cancellationToken = default)
        => InvokeAndAssertAsync(
              (cancellationToken) => TryGetSomethingAsync(cancellationToken),
              HttpStatusCode.OK,
              cancellationToken);

    public async Task<(MyDto Response, HttpResponseMessage Message)> TryGetSomethingAsync(
        CancellationToken cancellationToken = default)
            => TryGetAsJsonAsync<MyDto>("/v1/something", cancellationToken: cancellationToken);
    
    protected Task<TResponse> InvokeAndAssertAsync<TResponse>(
        Func<CancellationToken, Task<(TResponse? Response, HttpResponseMessage Message)>> invocationFunction,
        HttpStatusCode expectedStatusCode,
        CancellationToken cancellationToken = default)
        where TResponse : BaseResponse
            => _client.InvokeAndAssertAsync(
                    invocationFunction,
                    (invocationResult, _) => AssertResponse(invocationResult, expectedStatusCode),
                    cancellationToken);

    private TResponse AssertResponse<TResponse>(
        (TResponse? Response, HttpResponseMessage Message) result,
        HttpStatusCode expectedStatusCode)
            where TResponse : BaseResponse
    {
        result.Message.StatusCode.Should().Be(expectedStatusCode);

        result.Response.Should().NotBeNull();
        result.Response.Errors.Should().BeNull();
        result.Response.ErrorMessages.Should().BeNull();
        result.Response.IsSuccess.Should().BeTrue();
        result.Response.Status.Should().Be((int)expectedStatusCode);

        return result.Response;
    }
}

public sealed record MyDto(string Id);
```

### DI registration (optional)

```csharp
using Microsoft.Extensions.DependencyInjection;
using WhateverDotNet.API.Abstractions.Extensions;
using WhateverDotNet.API.Abstractions.Logging;

var services = new ServiceCollection();

services.AddApiLogMessageFormatter();
services.AddNUnitTestExecutionLogger(new TestExecutionLoggerOptions
{
    StepPattern = "STEP: {0}",
    PreconditionPattern = "PRE: {0}",
    ExpectedResultPattern = "EXP: {0}",
    ActualResultPattern = "ACT: {0}",
});
```

## Notes

- `BaseJsonRestClient` expects `_httpClient` to be initialized (for example in a `ConnectAs(...)` method). If it isn’t, calls throw `HttpClientNotInitializedException`.
- The default JSON serializer options use camelCase naming and are case-insensitive on property names.

