using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components;

public class DialogOptions : PaneOptions
{
    public object? Parent { get; set; }
}

public abstract class BaseDialog : BasePane
{
    protected BaseDialog(IScenarioContext scenarioContext, PaneOptions options)
        : base(scenarioContext, options)
    {
    }

    public override DialogOptions GetOptions()
    {
        return (DialogOptions)_options;
    }
}