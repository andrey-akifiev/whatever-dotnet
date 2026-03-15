using WhateverDotNet.TestApp.Components.Controls;
using WhateverDotNet.TestApp.Components.Pages;

namespace WhateverDotNet.TestApp.Components.Navigation;

public class NavigationMenuItemViewModel
    : HamburgerMenuItemViewModel<PageModel>
{
    public NavigationMenuItemViewModel(PageModel model)
        : base(model)
    {
    }
}