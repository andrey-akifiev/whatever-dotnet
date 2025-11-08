using WhateverDotNet.Components.Buttons;

namespace WhateverDotNet.Components.Selects;

public class SelectComponentOptions : ComponentOptions, ITextComponentOptions
{
    public ButtonComponentOptions[]? EmbeddedActions { get; set; }
    public bool? Expandable { get; set; }
    public string? XPathGroup { get; set; }
    public string? XPathInput { get; set; }
}