namespace WhateverDotNet.Components;

public class ComponentOptions
{
    public string? DisplayName { get; set; }

    public string? Name { get; set; }

    // TODO: rename
    // Handler invoked when a new item is created. Signature depends on your usage.
    public Func<object?>? NewItemHandler { get; set; }

    // TODO: rename
    // Side-effect callback. Signature depends on your usage.
    public Func<Task>? SideEffect { get; set; }

    // Can be an enum value (e.g. ComponentTypes) or a delegate/function.
    // Use a concrete type (ComponentTypes or Type/Delegate) if you prefer stricter typing.
    public object? Type { get; set; }

    /// <summary>
    /// Overrides XPath to the element. Used when XPath is dynamic and computed at runtime.
    /// </summary>
    public string? XPathComponent { get; set; }

    public string? XPathItem { get; set; }

    public string? XPathLoader { get; set; }

    public string? XPathParent { get; set; }

    public string? XPathValidation { get; set; }
}