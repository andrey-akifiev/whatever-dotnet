namespace WhateverDotNet.Components.Panes;

public class PaneOptions
{
    public string? BrowserTabTitle { get; set; }
    public string? Caption { get; set; }
    public IComponent[]? Components { get; set; }
    
    public string? Description { get; set; }
    public bool? IsTemplate { get; set; }

    public string? Name { get; set; }

    public string? OnLoadRequestAlias { get; set; }
    public string? OnLoadRequestEndpoint { get; set; }
    
    public string? OnSubmitRequestAlias { get; set; }
    public string? OnSubmitRequestEndpoint { get; set; }
    public string? OnSubmitVariableName { get; set; }

    public IApplicationPane[]? Panes { get; set; }
    
    public string? XPathCaption { get; set; }
    public string? XPathComponent { get; set; }
    public string? XPathLoader { get; set; }
    public string? XPathParent { get; set; }
}
