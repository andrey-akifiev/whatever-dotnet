namespace WhateverDotNet.TestApp.Components.Authentication;

/// <summary>
/// Service that performs OKTA sign-in (opens browser, handles redirect, returns access token).
/// </summary>
public interface IOktaSignInService
{
    /// <summary>
    /// Starts the OKTA sign-in flow: opens the default browser for user login,
    /// listens for the redirect, and returns the access token. Returns null if cancelled or failed.
    /// </summary>
    Task<string?> SignInAsync(CancellationToken cancellationToken = default);
}
