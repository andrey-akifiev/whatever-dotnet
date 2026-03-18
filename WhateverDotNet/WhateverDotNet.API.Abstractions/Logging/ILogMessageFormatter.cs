namespace WhateverDotNet.API.Abstractions.Logging;

/// <summary>
/// Formats structured log entries into a single human-readable message.
/// </summary>
public interface ILogMessageFormatter
{
    /// <summary>
    /// Formats the supplied entries into a log message.
    /// </summary>
    /// <param name="entries">Entries to format (order/meaning is formatter-specific).</param>
    /// <returns>Formatted log message.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entries"/> is <see langword="null"/> or empty.</exception>
    public string FormatLogMessage(params string?[] entries);
}