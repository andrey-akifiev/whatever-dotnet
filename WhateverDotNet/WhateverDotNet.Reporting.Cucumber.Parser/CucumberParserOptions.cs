namespace WhateverDotNet.Reporting.Cucumber.Parser;

public class CucumberParserOptions
{
    public string? BugIdTagPrefix { get; set; }
    
    public string? ReportsDirectory { get; set; }
    
    public string? ReportFileNamePattern { get; set; }
    
    public string? TestCaseIdTagPrefix { get; set; }
}