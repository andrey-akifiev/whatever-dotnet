using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Windows;

using WhateverDotNet.TestApp.Components.Navigation;
using WhateverDotNet.TestApp.Components.Pages;
using WhateverDotNet.TestApp.Excel.Commands;
using WhateverDotNet.TestApp.Excel.Pages.ReportSpecifications;
using WhateverDotNet.TestApp.Excel.ViewModels;
using WhateverDotNet.TestApp.Excel.Views;

namespace WhateverDotNet.TestApp.Excel.Playground
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<ICreateReportSpecificationCommand, CreateReportSpecificationCommand>();
                    services.AddSingleton<IRemoveReportSpecificationCommand, RemoveReportSpecificationCommand>();
                    services.AddSingleton<IUpdateReportSpecificationCommand, UpdateReportSpecificationCommand>();

                    services.AddSingleton<WhateverDotNet.TestApp.Excel.Stores.NavigationStore>();
                    
                    services.AddSingleton<Stores.ReportSpecificationsStore_New>();

                    services.AddTransient<ReportSpecificationsView>();
                    services.AddTransient<ReportSpecificationsViewModel>(CreateReportSpecificationsViewModel);
                    services.AddTransient<IPageViewModel, ReportSpecificationsViewModel>(CreateReportSpecificationsViewModel);

                    services.AddSingleton<MainViewModel>();

                    // Hamburger navigation: register page view models so they are discovered by NavigationStore

                    services.AddSingleton<INavigationStore, NavigationStore>();
                    services.AddSingleton<NavigationMenuViewModel>();
                    services.AddSingleton<HamburgerShellViewModel>();

                    services.AddSingleton<MainWindow>((services) => new MainWindow
                    {
                        DataContext = services.GetRequiredService<MainViewModel>(),
                    });
                })
                .Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }

        private ReportSpecificationsViewModel CreateReportSpecificationsViewModel(IServiceProvider services)
        {
            return ReportSpecificationsViewModel.LoadViewModel(
                services.GetRequiredService<Stores.ReportSpecificationsStore_New>());
        }
    }
}