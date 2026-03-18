using CommandLine;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using WhateverDotNet.Reporting.AzureDevOps.AzureDevOpsClient;
using WhateverDotNet.Reporting.AzureDevOps.Contracts;
using WhateverDotNet.Reporting.AzureDevOps.Extensions;
using WhateverDotNet.Reports.Abstractions;
using WhateverDotNet.Reports.Parser.Abstractions;
using WhateverDotNet.Reports.Parser.CSharp;

namespace WhateverDotNet.AzureDevOps.SyncTool;

public class Program
{
    class Options
    {
        [Option('a', "area", Required = true, HelpText = "Area path for the work items.")]
        public string? Area { get; set; }
        
        [Option("pat", Required = true, HelpText = "Personal Access Token for Azure DevOps.")]
        public string? PersonalAccessToken { get; set; }
        
        [Option("path", Required = false, HelpText = "Base path for the work items.")]
        public string? BasePath { get; set; }
        
        [Option('u', "url", Required = true, HelpText = "Azure DevOps organization URL.")]
        public string? BaseUrl { get; set; }
        
        [Option("project", Required = true, HelpText = "Azure DevOps project name.")]
        public string? ProjectName { get; set; }
    }
    
    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        
        builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        
        builder.Services.AddMemoryCache();

        builder.Services.AddCSharpTestResultsParser();
        builder.Services.AddSingleton<AzureDevOpsOptions>(
            new AzureDevOpsOptions
            {
                AreaPath = "TTO Assets\\SDP - MDM",
                BaseUrl = "https://dev.azure.com/DeloitteTaxTechnology",
                ProjectName = "TTO Assets",
                PersonalAccessToken = "",
            });
        builder.Services.AddAzureDevOpsSink();
        
        using IHost host = builder.Build();

        var testResultsParser = host.Services.GetRequiredService<ITestResultsParser>();
        string outputFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults/results.trx");
        
        WhateverTestCase[] testResults = (await testResultsParser
            .ParseTestCasesAsync(outputFilePath)
            .ConfigureAwait(false))
            .ToArray();
        
        var azureDevOps = host.Services.GetRequiredService<IAlmSink>();

        foreach (var testResult in testResults)
        {
            testResult.Attributes = new Dictionary<string, object>
            {
                { WorkItemStandardFields.AreaPath, "TTO Assets\\SDP - MDM" },
                { "Custom.Automation_Status", "Not Automated" },
                { "Custom.Release", "8.0 INCR" },
                { "Custom.TestDomain", "Core-IDB" },
                { "Custom.Test_Type", "Smoke" },
            };
        }

        await azureDevOps
            .SyncTestCasesAsync(new WhateverTestCaseSpecification
                {
                    StandardFields = [],
                    CustomFields =
                        [
                            "Custom.Automation_Status",
                            "Custom.Release",
                            "Custom.TestDomain",
                            "Custom.Test_Type",
                        ],
                },
                testResults,
                CancellationToken.None);

        (await Parser
                .Default
                .ParseArguments<Options>(args)
                .WithParsedAsync(RunWithOptions))
            .WithNotParsed(HandleParseError);
    }
    
    private static async Task RunWithOptions(Options opts)
    {
        throw new NotImplementedException();
    }
    
    private static void HandleParseError(IEnumerable<Error> errors)
    {
    }
}