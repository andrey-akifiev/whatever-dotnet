namespace WhateverDotNet.Components.Exceptions;

public class ComponentNotSpecifiedException : Exception
{
    public ComponentNotSpecifiedException()
        : base("No component has been specified in parent options.")
    {
    }
    
    public ComponentNotSpecifiedException(string componentName)
        : base($"Component '{componentName}' has not been specified in parent options.")
    {
    }
}