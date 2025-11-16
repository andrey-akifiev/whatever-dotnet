using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components.Panes;

public abstract class BasePane : BasePane<PaneOptions>, IApplicationPane
{
    protected BasePane(IScenarioContext scenarioContext, PaneOptions options)
        : base(scenarioContext, options)
    {
    }
}