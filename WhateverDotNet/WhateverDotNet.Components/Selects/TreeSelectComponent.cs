using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components.Selects;

public class TreeSelectComponent : BaseSelectComponent
{
    public TreeSelectComponent(IScenarioContext scenarioContext, SelectComponentOptions options)
        : base(scenarioContext, options)
    {
    }
}