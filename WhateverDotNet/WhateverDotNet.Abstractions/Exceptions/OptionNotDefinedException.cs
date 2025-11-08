namespace WhateverDotNet.Abstractions.Exceptions;

public class OptionNotDefinedException : Exception
{
    public OptionNotDefinedException(string optionName, object? parentObject = null)
        : base($"Option '{optionName}' is not defined for '{parentObject}'.")
    {
    }
}