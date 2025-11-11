using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;
using WhateverDotNet.Components.Selects;

namespace WhateverDotNet.Components;

public class RadioGroupComponent : BaseSelectComponent
{
    public RadioGroupComponent(IScenarioContext scenarioContext, SelectComponentOptions options)
        : base(scenarioContext, options)
    {
    }

    protected override string GetXPathItem(string? item = null)
    {
        if (string.IsNullOrWhiteSpace(GetOptions().XPathItem))
        {
            throw new OptionNotDefinedException(nameof(_options.XPathItem));
        }

        if (string.IsNullOrEmpty(item))
        {
            return $"{GetXPathComponent()}{GetOptions().XPathItem}";
        }
        
        return $"{GetXPathComponent()}{GetOptions().XPathItem}[contains(normalize-space(), \"{item}\")]";
    }
}