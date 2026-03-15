using Microsoft.Extensions.DependencyInjection;

using System.Collections.ObjectModel;
using WhateverDotNet.TestApp.Components.Pages;
using WhateverDotNet.TestApp.Components.ViewModels;

namespace WhateverDotNet.TestApp.Components.Navigation;

/// <summary>
/// Navigation store that discovers main pages from all registered <see cref="BasePageViewModel"/> descendants<br/>
/// and support pages from all registered <see cref="SupportPageViewModel"/> descendants in DI.
/// </summary>
public class NavigationStore : INavigationStore
{
    private readonly ObservableCollection<PageModel> _pages;
    private readonly ObservableCollection<PageModel> _supportPages;
    private readonly IServiceProvider _serviceProvider;
    private PageModel? _selectedPage;
    private BaseViewModel? _currentViewModel;

    public NavigationStore(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        // Main navigation: all IPageViewModel that are not SupportPageViewModel (build PageModels with ViewFactory)
        var mainViewModels = serviceProvider
            .GetServices<IPageViewModel>()
            .Where(vm => vm is not SupportPageViewModel<PageModel>);

        // Support: all SupportPageViewModel
        var supportViewModels = serviceProvider.GetServices<SupportPageViewModel<PageModel>>();

        _pages = new ObservableCollection<PageModel>(BuildPageModelsFromPageViewModels(mainViewModels));
        _supportPages = new ObservableCollection<PageModel>(BuildPageModels(supportViewModels));

        // Select first main page by default
        if (_pages.Count > 0)
        {
            SetCurrentPage(_pages[0]);
        }
    }

    public event Action? CurrentPageChanged;

    public ObservableCollection<PageModel> Pages => _pages;

    public ObservableCollection<PageModel> SupportPages => _supportPages;

    public PageModel? SelectedPage
    {
        get => _selectedPage;
        set => SetCurrentPage(value);
    }

    public BaseViewModel? CurrentViewModel => _currentViewModel;

    public void SetCurrentPage(PageModel? page)
    {
        if (ReferenceEquals(_selectedPage, page))
        {
            return;
        }

        _currentViewModel?.Dispose();
        _selectedPage = page;
        _currentViewModel = page?.ViewFactory?.Invoke();
        OnCurrentPageChanged();
    }

    private IEnumerable<PageModel> BuildPageModels(IEnumerable<BasePageViewModel<PageModel>> viewModels)
    {
        foreach (var vm in viewModels)
        {
            var type = vm.GetType();
            yield return new PageModel
            {
                Title = vm.Title ?? string.Empty,
                Icon = vm.Icon,
                ViewFactory = () => (BaseViewModel)_serviceProvider.GetRequiredService(type),
            };
        }
    }

    private IEnumerable<PageModel> BuildPageModelsFromPageViewModels(IEnumerable<IPageViewModel> viewModels)
    {
        foreach (var vm in viewModels)
        {
            var type = vm.GetType();
            yield return new PageModel
            {
                Title = vm.Title ?? string.Empty,
                Icon = vm.Icon,
                ViewFactory = () => (BaseViewModel)_serviceProvider.GetRequiredService(type),
            };
        }
    }

    protected virtual void OnCurrentPageChanged()
    {
        CurrentPageChanged?.Invoke();
    }
}