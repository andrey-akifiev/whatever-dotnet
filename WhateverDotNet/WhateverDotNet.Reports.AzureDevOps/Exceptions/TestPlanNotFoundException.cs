namespace WhateverDotNet.Reporting.AzureDevOps.Exceptions;

public class TestPlanNotFoundException : Exception
{
    public TestPlanNotFoundException(string testPlanName)
        : base($"Test Plan '{testPlanName}' not found.")
    {
    }

    public TestPlanNotFoundException(string projectName, string testPlanName)
        : base($"Test Plan '{testPlanName}' not found in project '{projectName}'.")
    {
    }
}