using System.Collections.ObjectModel;
using WhateverDotNet.TestApp.Components.Pages;
using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Navigation;

/// <summary>
/// Holds the list of navigation pages (from DI) and the currently selected page / view model.
/// </summary>
public interface INavigationStore
{
    /// <summary>
    /// Raised when the current page or view model changes.
    /// </summary>
    event Action? CurrentPageChanged;

    /// <summary>
    /// All navigation pages (main menu). Filled from DI at startup.
    /// </summary>
    ObservableCollection<PageModel> Pages { get; }

    /// <summary>
    /// Support pages shown at the bottom of the menu (e.g. Settings, Help).
    /// Populated from DI by resolving all registered <see cref="SupportPageViewModel"/> descendants.
    /// </summary>
    ObservableCollection<PageModel> SupportPages { get; }

    /// <summary>
    /// Currently selected page model, or null if none.
    /// </summary>
    PageModel? SelectedPage { get; set; }

    /// <summary>
    /// Current view model for the selected page (from SelectedPage.ViewFactory).
    /// Null if no selection or factory is null.
    /// </summary>
    BaseViewModel? CurrentViewModel { get; }

    /// <summary>
    /// Sets the current page and updates CurrentViewModel. Idempotent if same page.
    /// </summary>
    void SetCurrentPage(PageModel? page);
}