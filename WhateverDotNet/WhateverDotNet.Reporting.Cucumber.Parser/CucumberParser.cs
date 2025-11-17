using System.Text.Json;

using Microsoft.Extensions.Logging;

using WhateverDotNet.Cucumber.Json.Contracts;
using WhateverDotNet.Reporting.Contracts;

namespace WhateverDotNet.Reporting.Cucumber.Parser;

public class CucumberParser(CucumberParserOptions options, ILoggerFactory? loggerFactory = null)
    : ITestResultsParser
{
    private readonly ILogger<CucumberParser>? _logger = loggerFactory?.CreateLogger<CucumberParser>();
    private readonly CucumberParserOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    
    public async Task<IEnumerable<AgnosticTestResult>> ParseAsync(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<string> reportFilePaths = GetReportFilePaths();
        IEnumerable<Feature> features = 
            await ReadCucumberReportsAsync(reportFilePaths, cancellationToken)
                .ConfigureAwait(false);

        List<AgnosticTestResult> agnosticTestResults = new();
        
        foreach (Feature feature in features)
        {
            if (feature.Elements == null)
            {
                continue;
            }

            foreach (Scenario scenario in feature.Elements)
            {
                var errorMessages = scenario
                    .Steps
                    ?.Where(step => step.Result?.Status == "failed")
                    .Select(step => $"{step.Name}\n{step.Result?.ErrorMessage}\n")
                    .ToArray();
                string? errorMessage = errorMessages is not null && errorMessages.Any()
                    ? string.Join("\n", errorMessages)
                    : null;
                
                agnosticTestResults.Add(
                    new AgnosticTestResult
                    {
                        AutomationFeatureName = feature.Name,
                        AutomationTestName = scenario.Name,
                        BugIds = 
                            string.IsNullOrWhiteSpace(_options.BugIdTagPrefix)
                            ? null
                            : scenario
                                .Tags?
                                .Where(tag => tag is { Name: not null } && tag.Name.StartsWith(_options.BugIdTagPrefix))
                                .Select(tag => int.Parse(tag.Name!.Substring(_options.BugIdTagPrefix!.Length)))
                                .ToArray(),
                        DurationInMilliseconds = (scenario
                            .Steps?
                            .Sum(step => (long)(step.Result?.Duration ?? 0L)) ?? 0L) / 1000000,
                        ErrorMessage = errorMessage,
                        State = "Completed",
                        Status = scenario.Steps == null || scenario.Steps.Count == 0
                            ? "NotExecuted"
                            : scenario.Steps.Any(step => step.Result?.Status == "failed")
                                ? "Failed"
                                : "Passed",
                        TestCaseIds =
                            string.IsNullOrWhiteSpace(_options.TestCaseIdTagPrefix)
                            ? null
                            : scenario
                                .Tags?
                                .Where(tag => tag is { Name: not null } && tag.Name.StartsWith(_options.TestCaseIdTagPrefix))
                                .Select(tag => int.Parse(tag.Name!.Substring(_options.TestCaseIdTagPrefix!.Length)))
                                .ToArray(),
                    });
            }
        }
        
        return agnosticTestResults;
    }

    private IEnumerable<string> GetReportFilePaths()
    {
        if (!Directory.Exists(_options.ReportsDirectory))
        {
            throw new DirectoryNotFoundException(_options.ReportsDirectory);
        }
        
        return Directory.GetFiles(
            _options.ReportsDirectory,
            _options.ReportFileNamePattern!,
            SearchOption.TopDirectoryOnly);
    }
    
    private async Task<IEnumerable<Feature>> ReadCucumberReportsAsync(
        IEnumerable<string> reportFilePaths,
        CancellationToken cancellationToken = default)
    {
        List<Feature> features = new();
        
        foreach (string reportFilePath in reportFilePaths)
        {
            Feature[]? reportFeatures = 
                await ReadCucumberReportAsync(reportFilePath, cancellationToken)
                    .ConfigureAwait(false);
            if (reportFeatures is { Length: > 0 })
            {
                features.AddRange(reportFeatures);
            }
        }

        return features;
    }

    private async Task<Feature[]?> ReadCucumberReportAsync(
        string reportFilePath,
        CancellationToken cancellationToken = default)
    {
        string content = await File
            .ReadAllTextAsync(reportFilePath, cancellationToken)
            .ConfigureAwait(false);
        return JsonSerializer.Deserialize<Feature[]>(content);
    }
}