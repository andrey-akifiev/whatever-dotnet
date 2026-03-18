using System.Text;

using WhateverDotNet.API.Abstractions.Extensions;

namespace WhateverDotNet.API.Abstractions.Logging;

public class ApiLogMessageFormatter : ILogMessageFormatter
{
    public const int CallerClientMethodNameEntryIdx = 0;
    public const int HttpMethodNameEntryIdx = 1;
    public const int HttpEndpointPathEntryIdx = 2;
    public const int SerializedPayloadEntryIdx = 3;

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