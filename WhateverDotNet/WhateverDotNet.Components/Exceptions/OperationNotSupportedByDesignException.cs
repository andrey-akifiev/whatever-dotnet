namespace WhateverDotNet.Components.Exceptions;

public class OperationNotSupportedByDesignException(string operationName, string componentName) : Exception(
    $"Operation '{operationName}' is not supported by component '{componentName}' by its conceptual design.\nIf you need to execute specified operation, then you need to use another component.");