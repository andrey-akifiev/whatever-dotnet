using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WhateverDotNet.TestApp.Components.Authentication;

/// <summary>
/// OKTA sign-in using authorization code flow with PKCE: opens default browser,
/// listens for redirect on localhost, exchanges code for access token.
/// </summary>
public class OktaSignInService : IOktaSignInService
{
    private readonly OktaSignInOptions _options;
    private readonly AuthenticationStore _store;

    public OktaSignInService(OktaSignInOptions options, AuthenticationStore store)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public async Task<string?> SignInAsync(CancellationToken cancellationToken = default)
    {
        var redirectUri = $"http://localhost:{_options.RedirectPort}/callback";
        var (codeVerifier, codeChallenge) = GeneratePkce();

        var baseUrl = _options.OktaDomain.TrimEnd('/');
        var authUrl = $"{baseUrl}/v1/authorize?" + string.Join("&",
            $"client_id={Uri.EscapeDataString(_options.ClientId)}",
            "response_type=code",
            "scope=" + Uri.EscapeDataString(_options.Scope),
            "redirect_uri=" + Uri.EscapeDataString(redirectUri),
            "code_challenge=" + codeChallenge,
            "code_challenge_method=S256",
            "state=" + Uri.EscapeDataString(Guid.NewGuid().ToString("N")));

        using var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{_options.RedirectPort}/");
        listener.Start();

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = authUrl,
                UseShellExecute = true
            });

            string? code = null;
            var getContextTask = listener.GetContextAsync();
            using (cancellationToken.Register(listener.Stop))
            {
                var context = await getContextTask.ConfigureAwait(false);
                var request = context.Request;
                var response = context.Response;

                if (request.Url?.AbsolutePath.EndsWith("/callback", StringComparison.OrdinalIgnoreCase) == true)
                {
                    code = request.QueryString["code"];
                    var state = request.QueryString["state"];
                    var error = request.QueryString["error"];
                    if (!string.IsNullOrEmpty(error))
                    {
                        SendHtmlResponse(response, 400, $"Error: {WebUtility.HtmlEncode(error)}. You can close this window.");
                        return null;
                    }
                }

                var html = string.IsNullOrEmpty(code)
                    ? "<p>No authorization code received. You can close this window.</p>"
                    : "<p>Sign-in successful. You can close this window.</p>";
                SendHtmlResponse(response, 200, html);
            }

            if (string.IsNullOrEmpty(code))
                return null;

            var token = await ExchangeCodeForTokenAsync(baseUrl, redirectUri, code, codeVerifier, cancellationToken).ConfigureAwait(false);
            if (token != null)
                _store.AccessToken = token;
            return token;
        }
        finally
        {
            try { listener.Stop(); } catch { /* ignore */ }
        }
    }

    private static (string CodeVerifier, string CodeChallenge) GeneratePkce()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        var codeVerifier = Base64UrlEncode(bytes);
        var challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        var codeChallenge = Base64UrlEncode(challengeBytes);
        return (codeVerifier, codeChallenge);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static void SendHtmlResponse(HttpListenerResponse response, int statusCode, string body)
    {
        response.StatusCode = statusCode;
        response.ContentType = "text/html; charset=utf-8";
        var html = $"<!DOCTYPE html><html><head><meta charset=\"utf-8\"/><title>Sign in</title></head><body>{body}</body></html>";
        var buf = Encoding.UTF8.GetBytes(html);
        response.ContentLength64 = buf.Length;
        response.OutputStream.Write(buf, 0, buf.Length);
        response.OutputStream.Close();
    }

    private async Task<string?> ExchangeCodeForTokenAsync(
        string baseUrl,
        string redirectUri,
        string code,
        string codeVerifier,
        CancellationToken cancellationToken)
    {
        var tokenUrl = $"{baseUrl}/v1/token";
        using var client = new HttpClient();
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
            ["client_id"] = _options.ClientId,
            ["code_verifier"] = codeVerifier
        });

        var response = await client.PostAsync(tokenUrl, content, cancellationToken).ConfigureAwait(false);
        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
            return null;

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        if (root.TryGetProperty("access_token", out var accessTokenEl))
            return accessTokenEl.GetString();
        return null;
    }
}
