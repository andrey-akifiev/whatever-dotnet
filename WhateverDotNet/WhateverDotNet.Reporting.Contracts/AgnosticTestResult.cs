namespace WhateverDotNet.Reporting.Contracts;

public class AgnosticTestResult
{
    public string? AutomationFeatureName { get; set; }
    
    public string? AutomationTestName { get; set; }
    
    public IEnumerable<int>? BugIds { get; set; }
    
    public string? ConfigurationId { get; set; }
    
    public long? DurationInMilliseconds { get; set; }
    
    public string? ErrorMessage { get; set; }
    
    public string? State { get; set; }
    
    public string? Status { get; set; }
    
    public IEnumerable<int>? TestCaseIds { get; set; }
    
    public IEnumerable<int>? TestResultIds { get; set; }
    
    public int? TestRunId { get; set; }
    
    public string? TestType { get; set; }
}