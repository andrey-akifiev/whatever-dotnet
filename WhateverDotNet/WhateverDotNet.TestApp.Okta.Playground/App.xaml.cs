using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WhateverDotNet.TestApp.Components.Authentication;

namespace WhateverDotNet.TestApp.Okta.Playground;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<AuthenticationStore>();
                services.AddSingleton<OktaAuthViewModel>();
                services.AddSingleton<MainWindow>(sp => new MainWindow
                {
                    DataContext = sp.GetRequiredService<OktaAuthViewModel>()
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
        _host.StopAsync().GetAwaiter().GetResult();
        _host.Dispose();
        base.OnExit(e);
    }
}
