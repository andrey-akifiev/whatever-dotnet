namespace WhateverDotNet.Reports.Abstractions;

public interface IFieldValuesProvider
{
    public Task<string> GetFieldValueAsync(
        string fieldName,
        CancellationToken cancellationToken = default);
}