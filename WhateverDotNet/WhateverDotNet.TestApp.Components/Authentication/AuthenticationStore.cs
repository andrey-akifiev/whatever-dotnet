using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WhateverDotNet.TestApp.Components.Authentication;

/// <summary>
/// Holds the current authentication state (e.g. access token) for the application.
/// </summary>
public class AuthenticationStore : INotifyPropertyChanged
{
    private string? _accessToken;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Current access token from the identity provider (e.g. OKTA). Null when not signed in.
    /// </summary>
    public string? AccessToken
    {
        get => _accessToken;
        set
        {
            if (_accessToken == value)
                return;
            _accessToken = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSignedIn));
        }
    }

    /// <summary>
    /// True when <see cref="AccessToken"/> is non-null and non-empty.
    /// </summary>
    public bool IsSignedIn => !string.IsNullOrEmpty(_accessToken);

    public void SignOut()
    {
        AccessToken = null;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
