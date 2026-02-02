using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Configuration;
using System.Data;
using System.Windows;
using WhateverDotNet.TestApp.Excel.Commands;
using WhateverDotNet.TestApp.Excel.Stores;
using WhateverDotNet.TestApp.Excel.ViewModels;

namespace WhateverDotNet.TestApp.Playground
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

                    services.AddSingleton<ReportSpecificationsStore>();
                    services.AddSingleton<SelectedReportSpecificationStore>();

                    
                    services.AddSingleton<MainViewModel>();
                    services.AddTransient<ReportSpecificationsViewModel>(CreateReportSpecificationsViewModel);

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
            throw new NotImplementedException();
        }

    }
}