namespace WhateverDotNet.API.Abstractions.Extensions;

/// <summary>
/// String helpers used by the logging/formatting utilities in this package.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts a client method name into a human-readable endpoint name.
    /// Removes common prefixes/suffixes such as <c>Try</c> and <c>Async</c>,
    /// then splits PascalCase into regular words.
    /// </summary>
    /// <param name="methodName">Method name to convert.</param>
    /// <returns>A readable name, or <see cref="string.Empty"/> when the conversion produces no characters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="methodName"/> is <see langword="null"/> or whitespace.</exception>
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

    /// <summary>
    /// Splits a PascalCase/camelCase string into a regular spaced sentence.
    /// </summary>
    /// <param name="input">Input string.</param>
    /// <returns>The spaced string (or the original value if it is <see langword="null"/> or empty).</returns>
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