namespace WhateverDotNet.Reporting.AzureDevOps.Exceptions;

public class TestSuiteNotFoundException(string projectName, int testPlanId, string testSuiteName)
    : Exception($"Test suite '{testSuiteName}' not found in test plan '{testPlanId}' of project '{projectName}'.");