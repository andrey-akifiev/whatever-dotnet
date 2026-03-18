namespace WhateverDotNet.Reporting.AzureDevOps.Exceptions;

public class WorkItemTypeNotFoundException : Exception
{
    public WorkItemTypeNotFoundException(string workItemTypeName)
        : base($"Work item type '{workItemTypeName}' not found.")
    {
    }
    
    public WorkItemTypeNotFoundException(string projectName, string workItemTypeName)
        : base($"Work item type '{workItemTypeName}' not found in project '{projectName}'.")
    {
    }
}