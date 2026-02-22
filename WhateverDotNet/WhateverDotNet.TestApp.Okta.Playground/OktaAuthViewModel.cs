using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WhateverDotNet.TestApp.Components.Authentication;

namespace WhateverDotNet.TestApp.Okta.Playground;

public partial class OktaAuthViewModel : ObservableObject
{
    private readonly AuthenticationStore _store;

    [ObservableProperty]
    private string _oktaDomain = "https://your-domain.okta.com";

    [ObservableProperty]
    private string _clientId = "";

    [ObservableProperty]
    private string _redirectPort = "8412";

    [ObservableProperty]
    private string _scope = "openid profile";

    [ObservableProperty]
    private string _statusMessage = "";

    [ObservableProperty]
    private bool _isSigningIn;

    public OktaAuthViewModel(AuthenticationStore store)
    {
        _store = store;
        _store.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(AuthenticationStore.AccessToken))
                OnPropertyChanged(nameof(AccessToken));
        };
    }

    public string? AccessToken => _store.AccessToken;

    [RelayCommand(CanExecute = nameof(CanSignIn))]
    private async Task SignInAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(OktaDomain) || string.IsNullOrWhiteSpace(ClientId))
        {
            StatusMessage = "Please set OKTA Domain and Client ID.";
            return;
        }

        IsSigningIn = true;
        StatusMessage = "Opening browser… Complete sign-in in the browser, then return here.";
        try
        {
            if (!int.TryParse(RedirectPort?.Trim(), out var port) || port <= 0 || port > 65535)
                port = 8412;
            var options = new OktaSignInOptions
            {
                OktaDomain = OktaDomain.Trim(),
                ClientId = ClientId.Trim(),
                RedirectPort = port,
                Scope = Scope.Trim()
            };
            var service = new OktaSignInService(options, _store);
            var token = await service.SignInAsync(cancellationToken);
            if (token != null)
            {
                StatusMessage = "Sign-in successful.";
                OnPropertyChanged(nameof(AccessToken));
            }
            else
            {
                StatusMessage = "Sign-in was cancelled or failed. Check the browser and try again.";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsSigningIn = false;
        }
    }

    private bool CanSignIn() => !IsSigningIn;

    partial void OnIsSigningInChanged(bool value) => SignInCommand.NotifyCanExecuteChanged();
}
