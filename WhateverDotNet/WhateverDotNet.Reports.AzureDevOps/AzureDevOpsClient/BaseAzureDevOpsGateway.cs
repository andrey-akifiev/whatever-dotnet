using Microsoft.VisualStudio.Services.WebApi;

namespace WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

public abstract class BaseAzureDevOpsGateway<TDecoratedClient>(AzureDevOpsService azureDevOpsService)
    : IDisposable
    where TDecoratedClient : IVssHttpClient
{
    protected AzureDevOpsService AzureDevOpsService = azureDevOpsService 
                                                      ?? throw new ArgumentNullException(nameof(azureDevOpsService));
    
    private TDecoratedClient? _client;
    private readonly Lock _lock = new();

    protected TDecoratedClient Client
    {
        get
        {
            if (_client != null) return _client;

            lock (_lock)
            {
                return _client ??= CreateClient();
            }
        }
    }

    private TDecoratedClient CreateClient()
    {
        return AzureDevOpsService.Connection.GetClient<TDecoratedClient>();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}