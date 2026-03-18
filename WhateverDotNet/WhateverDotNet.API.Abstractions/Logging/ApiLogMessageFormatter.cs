using System.Text;

using WhateverDotNet.API.Abstractions.Extensions;

namespace WhateverDotNet.API.Abstractions.Logging;

/// <summary>
/// Default formatter used to create a readable API-call log message for test output.
/// </summary>
public class ApiLogMessageFormatter : ILogMessageFormatter
{
    /// <summary>
    /// Index of the caller client method name entry.
    /// </summary>
    public const int CallerClientMethodNameEntryIdx = 0;

    /// <summary>
    /// Index of the HTTP method name entry.
    /// </summary>
    public const int HttpMethodNameEntryIdx = 1;

    /// <summary>
    /// Index of the HTTP endpoint path entry.
    /// </summary>
    public const int HttpEndpointPathEntryIdx = 2;

    /// <summary>
    /// Index of the serialized request payload entry.
    /// </summary>
    public const int SerializedPayloadEntryIdx = 3;

    /// <inheritdoc />
    public string FormatLogMessage(params string?[] entries)
    {
        if (entries is null || entries.Length == 0)
        {
            throw new ArgumentNullException(nameof(entries));
        }
        
        string? callerClientMethodName = entries[CallerClientMethodNameEntryIdx];
        string? httpMethodName = entries[HttpMethodNameEntryIdx];
        string? endpointPath = entries[HttpEndpointPathEntryIdx];
        string? serializedPayload = entries[SerializedPayloadEntryIdx];
        
        StringBuilder sb = new();

        sb.Append("Perform API call ");

        if (callerClientMethodName is not null)
        {
            sb.Append($"to {callerClientMethodName.ToCleanEndpointName()} ");
        }

        sb.AppendLine("using:");
        
        sb.AppendLine($"{httpMethodName?.ToUpperInvariant()} {endpointPath}");

        if (serializedPayload is not null)
        {
            sb.AppendLine(serializedPayload);
        }
        
        sb.AppendLine();
        
        return sb.ToString();
    }
}