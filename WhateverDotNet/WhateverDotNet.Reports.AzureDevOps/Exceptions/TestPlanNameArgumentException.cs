namespace WhateverDotNet.Reporting.AzureDevOps.Exceptions;

public class TestPlanNameArgumentException(string argumentName)
    : ArgumentException("Test plan name cannot be null or whitespace.", argumentName);