using Microsoft.Extensions.DependencyInjection;

using WhateverDotNet.API.Abstractions.Logging;

namespace WhateverDotNet.API.Abstractions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNUnitTestExecutionLogger(
        this IServiceCollection services,
        TestExecutionLoggerOptions options)
    {
        services.AddSingleton(options);
        services.AddTransient<ITestExecutionLogger, NUnitTestExecutionLogger>();
        
        return services;
    }

    public static IServiceCollection AddApiLogMessageFormatter(this IServiceCollection services)
    {
        services.AddTransient<ILogMessageFormatter, ApiLogMessageFormatter>();
        
        return services;
    }
}