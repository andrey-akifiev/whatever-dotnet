namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public record WorkItemsRepositoryParameters(
    string ProjectName,
    string WorkItemType,
    string? AreaPath,
    string[]? FieldsToInclude = null);