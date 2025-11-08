using WhateverDotNet.Reporting.AzureDevOps.Repositories;
using WhateverDotNet.Reporting.Contracts;

namespace WhateverDotNet.Reporting.AzureDevOps.Handlers;

public class ConfigurationHandler(
    TestConfigurationsRepository testConfigurationsRepository,
    TestVariablesRepository testVariablesRepository,
    string[] azureDevOpsConfigurationKeys)
{
    private readonly TestConfigurationsRepository _testConfigurationsRepository = testConfigurationsRepository
        ?? throw new ArgumentNullException(nameof(testConfigurationsRepository));
    private readonly TestVariablesRepository _testVariablesRepository = testVariablesRepository
        ?? throw new ArgumentNullException(nameof(testVariablesRepository));

    private readonly string[]? _azureDevOpsConfigurationKeys = azureDevOpsConfigurationKeys;

    public async Task HandleAsync(
        AgnosticTestResult testResult,
        IReadOnlyDictionary<string, string> testRunConfigurations,
        CancellationToken cancellationToken = default)
    {
        foreach ((string testConfigurationVariable, string testConfigurationValue) in testRunConfigurations)
        {
            var existingTestVariable = await _testVariablesRepository
                .GetTestVariableByNameAsync(testConfigurationVariable, cancellationToken)
                .ConfigureAwait(false) 
                ?? await _testVariablesRepository
                    .CreateTestVariableAsync(
                        testConfigurationVariable,
                        values: [testConfigurationValue],
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

            if (!existingTestVariable.Values?.Contains(testConfigurationValue) ?? true)
            {
                List<string> updatedValues = existingTestVariable.Values != null
                    ? existingTestVariable.Values.ToList()
                    : new List<string>();
                updatedValues.Add(testConfigurationValue);
                existingTestVariable = await _testVariablesRepository
                    .UpdateTestVariableAsync(
                        testConfigurationVariable,
                        testConfigurationVariable,
                        values: updatedValues,
                        cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        var filteredConfigurations =
            _azureDevOpsConfigurationKeys == null || _azureDevOpsConfigurationKeys.Length == 0
                ? testRunConfigurations
                : testRunConfigurations
                    .Where(trc => !_azureDevOpsConfigurationKeys.Contains(trc.Key))
                    .ToDictionary();

        var existingConfigurations = await _testConfigurationsRepository
            .GetTestConfigurationsAsync(cancellationToken)
            .ConfigureAwait(false);

        var existingConfiguration = existingConfigurations
            .FirstOrDefault(ec =>
                ec.Values != null
                && ec.Values.Count == filteredConfigurations.Count
                && !ec.Values.Except(filteredConfigurations).Any())
            ?? await _testConfigurationsRepository
                .CreateTestConfigurationAsync(
                    "Auto-generated configuration",
                    filteredConfigurations,
                    cancellationToken)
                .ConfigureAwait(false);
        
        testResult.ConfigurationId = existingConfiguration.Id;
    }
}