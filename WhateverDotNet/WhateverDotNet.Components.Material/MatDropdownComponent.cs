using WhateverDotNet.Abstractions;
using WhateverDotNet.Components.Selects;

namespace WhateverDotNet.Components.Material;

public class MatDropdownComponent : DropdownComponent, ISelectComponent
{
    public MatDropdownComponent(IScenarioContext scenarioContext, SelectComponentOptions options)
        : base(
            scenarioContext,
            new SelectComponentOptions
            {
                DisplayName = options.DisplayName,
                EmbeddedActions = options.EmbeddedActions,
                Expandable = options.Expandable ?? true,
                Name = options.Name,
                NewItemHandler = options.NewItemHandler,
                SideEffect = options.SideEffect,
                Type = options.Type,
                XPathComponent = options.XPathComponent,
                XPathGroup = options.XPathGroup,
                XPathInput = options.XPathInput ?? "//input",
                XPathItem =  options.XPathItem ?? "//mat-option//span",
                XPathLoader = options.XPathLoader,
                XPathParent = options.XPathParent,
                XPathValidation = options.XPathValidation ?? "//mat-error",
            })
    {
    }
}