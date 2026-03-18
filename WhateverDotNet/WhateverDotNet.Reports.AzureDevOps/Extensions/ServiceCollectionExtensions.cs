using Microsoft.Extensions.DependencyInjection;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reporting.AzureDevOps.Repositories;
using WhateverDotNet.Reports.Abstractions;

namespace WhateverDotNet.Reporting.AzureDevOps.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAzureDevOpsSink(this IServiceCollection services)
    {
        services.AddSingleton<AzureDevOpsService>();
        
        services.AddSingleton<AzureTestPlansGateway>();
        services.AddSingleton<AzureTestResultsGateway>();
        services.AddSingleton<AzureWorkItemsGateway>();

        services.AddSingleton<TestConfigurationsRepository>();
        services.AddSingleton<TestPlansRepository>();
        services.AddSingleton<TestSuitesRepository>();
        services.AddSingleton<TestVariablesRepository>();
        services.AddSingleton<WorkItemsRepository>();
        services.AddSingleton<WorkItemTypesRepository>();
        services.AddSingleton<TestCasesRepository>();
        
        services.AddSingleton<IAlmSink, AzureDevOpsSink>();
        
        return services;
    }
}