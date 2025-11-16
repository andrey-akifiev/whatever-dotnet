using WhateverDotNet.Components.Buttons;

namespace WhateverDotNet.Components.Selects;

public class SelectComponentOptions : ComponentOptions, ITextComponentOptions
{
    public ButtonComponentOptions[]? EmbeddedActions { get; set; }
    public bool? Expandable { get; set; }
    public string? XPathGroup { get; set; }
    public string? XPathInput { get; set; }

    public SelectComponentOptions CombineWithDefaults(SelectComponentOptions defaults)
    {
        return new SelectComponentOptions
        {
            DisplayName = DisplayName ?? defaults.DisplayName,
            EmbeddedActions = EmbeddedActions ?? defaults.EmbeddedActions,
            Expandable = Expandable ?? defaults.Expandable,
            Name = Name ?? defaults.Name,
            NewItemHandler = NewItemHandler ?? defaults.NewItemHandler,
            SideEffect = SideEffect ?? defaults.SideEffect,
            Type = Type ?? defaults.Type,
            XPathComponent = XPathComponent ?? defaults.XPathComponent,
            XPathGroup = XPathGroup ?? defaults.XPathGroup,
            XPathInput = XPathInput ?? defaults.XPathInput,
            XPathItem = XPathItem ?? defaults.XPathItem,
            XPathLoader = XPathLoader ?? defaults.XPathLoader,
            XPathParent = XPathParent ?? defaults.XPathParent,
            XPathValidation = XPathValidation ?? defaults.XPathValidation,
        };
    }
}