using WhateverDotNet.TestApp.Components.Controls;
using WhateverDotNet.TestApp.Components.Stores;
using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Pages;

public class PageModel : ICloneable, IHamburgerMenuItemModel, IModel
{
    public PageModel()
    {
        Id = Guid.NewGuid();
    }

    public string? Description { get; init; }

    public string? Icon { get; init; }

    public Guid Id { get; set; }

    public string Title { get; init; }

    public Func<BaseViewModel>? ViewFactory { get; init; }

    public object Clone()
    {
        return new PageModel
        {
            Description = Description,
            Icon = Icon,
            Title = Title,
            ViewFactory = ViewFactory,
        };
    }
}