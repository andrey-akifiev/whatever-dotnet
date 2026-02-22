namespace WhateverDotNet.TestApp.Components.Authentication;

/// <summary>
/// Configuration for OKTA sign-in (authorization code flow with PKCE).
/// </summary>
public class OktaSignInOptions
{
    /// <summary>
    /// OKTA domain (e.g. "https://your-domain.okta.com" or "https://your-domain.okta.com/oauth2/default").
    /// </summary>
    public string OktaDomain { get; set; } = string.Empty;

    /// <summary>
    /// OIDC application client id.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Port for the local redirect URI (e.g. 8412). Redirect URI will be http://localhost:{RedirectPort}/callback.
    /// </summary>
    public int RedirectPort { get; set; } = 8412;

    /// <summary>
    /// Scopes to request (space-separated), e.g. "openid profile email".
    /// </summary>
    public string Scope { get; set; } = "openid profile";
}
