namespace WhateverDotNet.Reporting.AzureDevOps.Repositories;

public class WorkItemCustomFieldValue
{
    private readonly string? _value;
    private readonly Func<string>? _valueFactory;

    private WorkItemCustomFieldValue(string referenceKey)
    {
        if (string.IsNullOrWhiteSpace(referenceKey))
        {
            throw new ArgumentException("Reference key cannot be null or whitespace.", nameof(referenceKey));
        }
        
        ReferenceKey = referenceKey;
    }
    
    public WorkItemCustomFieldValue(string referenceKey, string value)
        : this(referenceKey)
    {
        _value = value;
    }

    public WorkItemCustomFieldValue(string referenceKey, Func<string> valueFactory)
        : this(referenceKey)
    {
        _valueFactory = valueFactory;
    }
    
    public string ReferenceKey { get; private set; }

    public string? GetValue()
    {
        return _value ?? _valueFactory?.Invoke();
    }
}