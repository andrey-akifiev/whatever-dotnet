using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;

public class AzureDevOpsService : IDisposable
{
    private readonly VssConnection _connection;
    private readonly ILogger? _logger;
    private readonly AzureDevOpsOptions _options;

    private readonly Lazy<AzureTestPlansService> _azureTestPlansService;
    private readonly Lazy<AzureTestResultsService> _azureTestResultsService;
    private readonly Lazy<AzureWorkItemsService> _azureWorkItemsService;

    public AzureDevOpsService(AzureDevOpsOptions options, ILoggerFactory? loggerFactory = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = loggerFactory?.CreateLogger<AzureDevOpsService>();
        _connection = new VssConnection(
            new Uri(_options.BaseUrl
                    ?? throw new ArgumentNullException(_options.BaseUrl)),
            new VssBasicCredential(
                string.Empty, 
                _options.PersonalAccessToken
                    ?? throw new ArgumentNullException(_options.PersonalAccessToken)));
        _azureTestPlansService = new Lazy<AzureTestPlansService>(
            () => new AzureTestPlansService(_connection, loggerFactory));
        _azureTestResultsService = new Lazy<AzureTestResultsService>(
            () => new AzureTestResultsService(_connection, loggerFactory));
        _azureWorkItemsService = new Lazy<AzureWorkItemsService>(
            () => new AzureWorkItemsService(_connection, loggerFactory));
    }
    
    public Task<TestConfiguration> CreateTestConfigurationAsync(
        string configurationName,
        string? description = null,
        bool isDefault = false,
        IReadOnlyDictionary<string, string>? values = null,
        CancellationToken cancellationToken = default)
    {
        return _azureTestPlansService
            .Value
            .CreateTestConfigurationAsync(
                _options.ProjectName!,
                configurationName,
                description,
                isDefault,
                values,
                cancellationToken);
    }
    
    public Task<TestVariable> CreateTestVariableAsync(
        string variableName,
        string? description = null,
        IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        return _azureTestPlansService
            .Value
            .CreateTestVariableAsync(
                _options.ProjectName!,
                variableName,
                description,
                values,
                cancellationToken);
    }

    public Task<IEnumerable<TestConfiguration>> GetTestConfigurationsAsync(
        CancellationToken cancellationToken = default)
    {
        return _azureTestPlansService
            .Value
            .GetTestConfigurationsAsync(_options.ProjectName!, cancellationToken);
    }

    public Task<IEnumerable<TestVariable>> GetTestVariablesAsync(CancellationToken cancellationToken = default)
    {
        return _azureTestPlansService
            .Value
            .GetTestVariablesAsync(_options.ProjectName!, cancellationToken);
    }

    public Task<TestVariable> UpdateTestVariableAsync(
        int testVariableId,
        string newName,
        string? description = null,
            IEnumerable<string>? values = null,
        CancellationToken cancellationToken = default)
    {
        return _azureTestPlansService
            .Value
            .UpdateTestVariableAsync(
                _options.ProjectName!,
                testVariableId,
                newName,
                description,
                values,
                cancellationToken);
    }
    
    public void Dispose()
    {
        if (_azureTestPlansService.IsValueCreated)
        {
            _azureTestPlansService.Value.Dispose();
        }

        if (_azureTestResultsService.IsValueCreated)
        {
            _azureTestResultsService.Value.Dispose();
        }
        
        if (_azureWorkItemsService.IsValueCreated)
        {
            _azureWorkItemsService.Value.Dispose();
        }

        _connection.Dispose();
    }
}