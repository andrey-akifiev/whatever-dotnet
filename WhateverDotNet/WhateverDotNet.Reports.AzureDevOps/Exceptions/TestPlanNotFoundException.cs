namespace WhateverDotNet.Reporting.AzureDevOps.Exceptions;

public class TestPlanNotFoundException(string projectName, string testPlanName)
    : Exception($"Test Plan '{testPlanName}' not found in project '{projectName}'.");