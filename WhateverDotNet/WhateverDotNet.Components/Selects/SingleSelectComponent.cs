using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Components.Selects
{
    public class SingleSelectComponent : BaseSelectComponent
    {
        protected static readonly SelectComponentOptions DefaultOptions = new SelectComponentOptions
        {
            XPathGroup = "//mat-accordion//mat-expansion-panel-header",
            XPathInput = "//input",
            XPathItem = "//mat-accordion//div[contains(@class, \"mat-expansion-panel-body\")]//span",
        };

        public SingleSelectComponent(IScenarioContext scenarioContext, SelectComponentOptions options)
            : base(scenarioContext, options.CombineWithDefaults(DefaultOptions))
        {
        }
    }
}