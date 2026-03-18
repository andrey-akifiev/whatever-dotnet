namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class WorkItemCustomFields
{
    public Dictionary<string, WorkItemCustomFieldValue> Fields { get; set; } = new();
}