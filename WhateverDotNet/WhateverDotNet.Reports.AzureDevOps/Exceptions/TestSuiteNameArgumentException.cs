namespace WhateverDotNet.Reporting.AzureDevOps.Exceptions;

public class TestSuiteNameArgumentException(string argumentName)
    : ArgumentException("Test suite name cannot be null or whitespace.", argumentName);