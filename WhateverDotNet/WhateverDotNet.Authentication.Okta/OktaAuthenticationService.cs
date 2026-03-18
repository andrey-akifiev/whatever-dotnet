using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Web;

using Okta.Auth.Sdk;
using Okta.Sdk.Abstractions.Configuration;

namespace WhateverDotNet.Authentication.Okta;

public class OktaAuthenticationService : IDisposable
{
    private HttpClient? _httpClient;
    
    private OktaAuthenticationOptions _options;

    public OktaAuthenticationService(OktaAuthenticationOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    private HttpClient HttpClient
    {
        get
        {
            if (_httpClient == null)
            {
                HttpClientHandler handler = new()
                {
                    AllowAutoRedirect = false,
                };
                _httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri(_options.OktaDomain!),
                };
            }

            return _httpClient;
        }
    }

    public async Task<string> ConnectAsAsync(string username, string password)
    {
        string sessionToken = await GetSessionTokenAsync(username, password).ConfigureAwait(false);
        
        (string verifier, string challenge) = GeneratePkceCodes();
        
        string authCode = await GetAuthCodeAsync(sessionToken, challenge).ConfigureAwait(false);
        
        string accessToken = await GetAuthTokenAsync(authCode, verifier).ConfigureAwait(false);
        
        return accessToken;
    }
    
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
    
    private async Task<string> GetAuthCodeAsync(string sessionToken, string challenge)
    {
        Dictionary<string, string> parameters = new()
        {
            { "client_id", _options.ClientId! },
            { "code_challenge", challenge },
            { "code_challenge_method", "S256" },
            { "nonce", Guid.NewGuid().ToString() },
            { "prompt", "none" },
            { "redirect_uri", $"{_options.OktaDomain!}enduser/callback" },
            { "response_type", "code" },
            { "scope", "openid profile email" },
            { "sessionToken", sessionToken },
            { "state", Guid.NewGuid().ToString() },
        };
        
        HttpResponseMessage response = await HttpClient
            .GetAsync($"/oauth2/v1/authorize?{ToQueryString(parameters)}")
            .ConfigureAwait(false);
        
        string? location = response.Headers.Location?.ToString();
        
        if (string.IsNullOrWhiteSpace(location))
        {
            throw new InvalidOperationException("Authorization failed: missing redirect location.");
        }
        
        string? authCode = HttpUtility
            .ParseQueryString(new Uri(location).Query)
            .Get("code");

        if (string.IsNullOrWhiteSpace(authCode))
        {
            throw new InvalidOperationException("Authorization failed: missing authorization code.");
        }
        
        return authCode;
    }
    
    private async Task<string> GetAuthTokenAsync(string authCode, string verifier)
    {
        Dictionary<string, string> tokenRequest = new()
        {
            { "client_id", _options.ClientId! },
            { "code", authCode },
            { "code_verifier", verifier },
            { "grant_type", "authorization_code" },
            { "redirect_uri", $"{_options.OktaDomain!}enduser/callback" },
        };
        
        HttpResponseMessage response = await HttpClient
            .PostAsync(
                "/oauth2/v1/token",
                new FormUrlEncodedContent(tokenRequest))
            .ConfigureAwait(false);
        
        AuthorizeResponse? result = await response
            .Content
            .ReadFromJsonAsync<AuthorizeResponse>()
            .ConfigureAwait(false);
        
        return result!.AccessToken!;
    }
    
    private async Task<string> GetSessionTokenAsync(string username, string password)
    {
        AuthenticationClient oktaClient = new(new OktaClientConfiguration
        {
            OktaDomain = _options.OktaDomain!,
        });

        AuthenticateOptions authOptions = new()
        {
            Username = username,
            Password = password,
        };
        
        var response = await oktaClient
            .AuthenticateAsync(authOptions)
            .ConfigureAwait(false);
        
        return response.SessionToken!;
    }
    
    private static (string verifier, string challenge) GeneratePkceCodes()
    {
        byte[] bytes = new byte[32];
        
        RandomNumberGenerator.Fill(bytes);
        
        string verifier = ToBase64String(bytes);

        using SHA256 sha256 = SHA256.Create();
        
        byte[] hash = sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(verifier));
        string challenge = ToBase64String(hash);

        return (verifier, challenge);
    }

    private static string ToBase64String(byte[] src)
    {
        return Convert
            .ToBase64String(src)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
    
    private static string ToQueryString(Dictionary<string, string> parameters)
    {
        return string.Join("&", parameters.Select(kvp =>
            $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));
    }
}