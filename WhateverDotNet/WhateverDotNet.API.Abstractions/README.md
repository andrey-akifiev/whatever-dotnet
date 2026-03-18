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
using Microsoft.Extensions.Logging;
using WhateverDotNet.API.Abstractions;
using WhateverDotNet.API.Abstractions.Logging;

public sealed class MyApiClient : BaseJsonRestClient
{
    public MyApiClient(
        BaseJsonRestClientOptions options,
        ILogMessageFormatter formatter,
        ITestExecutionLogger? apiLogger,
        ILoggerFactory? loggerFactory = null)
        : base(options, formatter, apiLogger, loggerFactory)
    {
    }

    public void ConnectAs(string bearerToken)
    {
        // The base class throws if BaseUrl/token are missing.
        _httpClient = CreateClient($"Bearer {bearerToken}");
    }

    public Task<MyDto> GetSomethingAsync(CancellationToken ct = default)
        => ExecuteAndAssertAsync(
            () => TryGetAsJsonAsync<MyDto>("/v1/something", cancellationToken: ct),
            HttpStatusCode.OK);

    protected override async Task<TResponse> ExecuteAndAssertAsync<TResponse>(
        Func<Task<(TResponse? Response, HttpResponseMessage Message)>> function,
        HttpStatusCode expectedStatusCode)
    {
        var (response, message) = await function().ConfigureAwait(false);

        if (message.StatusCode != expectedStatusCode)
        {
            var body = message.Content is null
                ? null
                : await message.Content.ReadAsStringAsync().ConfigureAwait(false);

            throw new InvalidOperationException(
                $"Expected {(int)expectedStatusCode} but got {(int)message.StatusCode}. Body: {body}");
        }

        return response!;
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

