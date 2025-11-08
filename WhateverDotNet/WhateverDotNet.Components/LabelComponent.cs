using WhateverDotNet.Abstractions;
using WhateverDotNet.Components.Exceptions;

namespace WhateverDotNet.Components;

public class LabelComponent : BaseComponent<ComponentOptions>
{
    public LabelComponent(IScenarioContext scenarioContext, ComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    public override Task<string> TypeAsync(string text, ForceOptions? options = null)
    {
        throw new OperationNotSupportedByDesignException(nameof(TypeAsync), nameof(LabelComponent));
    }
}