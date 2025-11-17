using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

public class AzureWorkItemsService(VssConnection vssConnection, ILoggerFactory? loggerFactory) : IDisposable
{
    private readonly ILogger<AzureTestPlansService>? _logger = loggerFactory?.CreateLogger<AzureTestPlansService>();
    private readonly WorkItemTrackingHttpClient? _workItemTrackingHttpClient =
        vssConnection == null
            ? throw new ArgumentNullException(nameof(vssConnection))
            : vssConnection.GetClient<WorkItemTrackingHttpClient>();
    
    public void Dispose()
    {
        _workItemTrackingHttpClient?.Dispose();
    }
    
}