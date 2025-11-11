using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components;

public class CheckboxComponent : BaseComponent<ComponentOptions>
{
    public CheckboxComponent(IScenarioContext scenarioContext, ComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    public override ComponentOptions GetOptions()
    {
        return _options;
    }
}