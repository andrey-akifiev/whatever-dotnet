namespace WhateverDotNet.Components.Buttons;

public class ButtonComponentOptions : ComponentOptions
{
    public Func<Task>? OnClickAsync { get; set; }
    public string? OnClickRequestAlias { get; set; }
    public string? OnClickRequestEndpoint { get; set; }
    public Func<Task>? OnClickValidationAsync { get; set; }
    public string? OnClickVariableName { get; set; }
}