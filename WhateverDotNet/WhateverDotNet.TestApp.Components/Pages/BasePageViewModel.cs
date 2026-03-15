using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Pages;

/// <summary>
/// Base class for all main navigation pages. Register descendants in DI as <see cref="BasePageViewModel"/><br/>
/// so the navigation store can discover them via <see cref="Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetServices{T}(IServiceProvider)"/>.
/// </summary>
public abstract class BasePageViewModel<TPageModel>
    : BaseViewModel,
      IPageViewModel
        where TPageModel : PageModel
{
    protected BasePageViewModel(TPageModel model)
    {
        Model = model;
    }

    public string? Description => Model.Description;

    /// <summary>
    /// Optional SVG path data or icon key for the menu item.
    /// </summary>
    public string? Icon => Model.Icon;

    public TPageModel Model { get; }

    /// <summary>
    /// Display title shown in the hamburger menu.
    /// </summary>
    public string Title => Model.Title;
}