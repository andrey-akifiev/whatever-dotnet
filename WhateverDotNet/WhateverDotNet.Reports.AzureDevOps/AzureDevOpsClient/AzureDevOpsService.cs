using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

public class AzureDevOpsService : IDisposable
{
    private readonly VssConnection _connection;
    private readonly AzureDevOpsOptions _options;

    public AzureDevOpsService(AzureDevOpsOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _connection = new VssConnection(
            new Uri(_options.BaseUrl
                    ?? throw new ArgumentNullException(_options.BaseUrl)),
            new VssBasicCredential(
                string.Empty, 
                _options.PersonalAccessToken
                    ?? throw new ArgumentNullException(_options.PersonalAccessToken)));
    }
    
    public VssConnection Connection => _connection;

    public void Dispose()
    {
        _connection.Dispose();
    }
}