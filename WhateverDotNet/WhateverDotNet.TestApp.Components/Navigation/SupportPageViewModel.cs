using WhateverDotNet.TestApp.Components.Pages;

namespace WhateverDotNet.TestApp.Components.Navigation;

/// <summary>
/// Base class for support pages (e.g. Settings, Help). Register descendants in DI as <see cref="SupportPageViewModel"/><br/>
/// so the navigation store can discover them and populate the support menu list.
/// </summary>
public abstract class SupportPageViewModel<TPageModel>
    : BasePageViewModel<TPageModel>
        where TPageModel : PageModel
{
    protected SupportPageViewModel(TPageModel model)
        : base(model)
    {
    }
}