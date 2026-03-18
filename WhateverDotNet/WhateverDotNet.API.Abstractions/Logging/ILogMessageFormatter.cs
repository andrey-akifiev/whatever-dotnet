namespace WhateverDotNet.API.Abstractions.Logging;

public interface ILogMessageFormatter
{
    public string FormatLogMessage(params string?[] entries);
}