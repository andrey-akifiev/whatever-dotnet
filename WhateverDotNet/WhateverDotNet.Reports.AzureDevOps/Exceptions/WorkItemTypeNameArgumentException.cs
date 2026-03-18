namespace WhateverDotNet.Reporting.AzureDevOps.Exceptions;

public class WorkItemTypeNameArgumentException(string argumentName)
    : ArgumentException("Work item type name cannot be null or whitespace.", argumentName);