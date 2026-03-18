namespace WhateverDotNet.API.Abstractions.Extensions;

public static class StringExtensions
{
    public static string ToCleanEndpointName(this string? methodName)
    {
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new ArgumentNullException(nameof(methodName));
        }
        
        methodName = methodName.Trim();

        if (methodName.StartsWith("Try", StringComparison.InvariantCultureIgnoreCase) && methodName.Length > 3)
        {
            methodName = methodName[3..];
        }
        
        if (methodName.EndsWith("Async", StringComparison.InvariantCultureIgnoreCase) && methodName.Length > 5)
        {
            methodName = methodName[..^5];
        }

        if (methodName.Length == 0)
        {
            return string.Empty;
        }
        
        return methodName.ToRegularCase();
    }

    public static string ToRegularCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        Span<char> buffer = stackalloc char[input.Length * 2];
        int pos = 0;

        buffer[pos++] = input[0];

        for (int i = 1; i < input.Length; i++)
        {
            char c = input[i];

            if (char.IsUpper(c))
            {
                buffer[pos++] = ' ';
            }

            buffer[pos++] = c;
        }

        return new string(buffer.Slice(0, pos));
    }
}