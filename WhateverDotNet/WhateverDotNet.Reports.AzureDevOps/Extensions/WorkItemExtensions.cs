using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using WhateverDotNet.Reporting.AzureDevOps.Contracts;

namespace WhateverDotNet.Reporting.AzureDevOps.Extensions;

public static class WorkItemExtensions
{
    public static string GetWorkItemType(this WorkItem workItem)
    {
        return workItem.Fields[WorkItemStandardFields.WorkItemType]?.ToString()
            ?? throw new ArgumentException($"Field {WorkItemStandardFields.WorkItemType} is not defined");
    }
}