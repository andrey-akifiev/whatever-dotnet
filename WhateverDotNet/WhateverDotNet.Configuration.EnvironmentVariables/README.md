# WhateverDotNet.Configuration.EnvironmentVariables

Builds strongly-typed configuration objects from environment variables.

## How it works

`EnvironmentVariablesProvider<TVariables>` reads writable public properties on `TVariables`, maps each property name to an environment-variable name (snake case, upper), then populates an instance:

- It first tries a environment-specific variable:
  - `"{ProjectPrefix}{PROPERTY_NAME}_{ENV_POSTFIX}"`
- If that is missing, it falls back to a non-environment-specific variable:
  - `"{ProjectPrefix}{PROPERTY_NAME}"`

Values are cached per `TestEnvironment`.

## Example

```csharp
using WhateverDotNet.Configuration;

public class MyVars
{
    public string? ApiBaseUrl { get; set; }
}

var options = new EnvironmentOptions
{
    ProjectPrefix = "MYAPP_"
};

var provider = new EnvironmentVariablesProvider<MyVars>(options);
var vars = provider.Get(TestEnvironment.QA);
```

If `MyVars.ApiBaseUrl` is mapped to `API_BASE_URL`, then the provider will look for:

- `MYAPP_API_BASE_URL_QA`
- (fallback) `MYAPP_API_BASE_URL`

