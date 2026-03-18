namespace WhateverDotNet.Reports.Parser.CSharp.Trx;

public class TrxTestResult
{
    public string? TestName { get; init; }
    public string? ClassName { get; init; }
    public string? Outcome { get; init; }
    public string? StdOut { get; init; }
    public string? StdErr { get; init; }
}