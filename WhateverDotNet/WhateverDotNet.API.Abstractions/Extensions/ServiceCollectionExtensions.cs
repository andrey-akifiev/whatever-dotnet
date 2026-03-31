using Microsoft.Extensions.DependencyInjection;

using WhateverDotNet.API.Abstractions.Logging;

namespace WhateverDotNet.API.Abstractions.Extensions;

/// <summary>
/// Dependency-injection helpers for registering abstractions from this package.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers an <see cref="ITestExecutionLogger"/> implementation that writes to standard output (NUnit-friendly).
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="options">Logger formatting options.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddNUnitTestExecutionLogger(
        this IServiceCollection services,
        TestExecutionLoggerOptions options)
    {
        services.AddSingleton(options);
        services.AddScoped<ITestExecutionLogger, NUnitTestExecutionLogger>();
        
        return services;
    }

    /// <summary>
    /// Registers the default <see cref="ILogMessageFormatter"/> used for API-call step logging.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddApiLogMessageFormatter(this IServiceCollection services)
    {
        services.AddTransient<ILogMessageFormatter, ApiLogMessageFormatter>();
        
        return services;
    }
}