using Microsoft.Extensions.DependencyInjection;

using WhateverDotNet.Reports.Parser.Abstractions;

namespace WhateverDotNet.Reports.Parser.CSharp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCSharpTestResultsParser(this IServiceCollection services)
    {
        services.AddSingleton<ITestResultsParser, CSharpParser>();
        return services;
    }
}