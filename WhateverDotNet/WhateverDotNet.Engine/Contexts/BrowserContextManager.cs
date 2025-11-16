using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using System.Collections.Concurrent;

using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Engine.Contexts
{
    public class BrowserContextManager : IAsyncDisposable
    {
        private readonly string _browserName;

        private readonly ILogger<BrowserContextManager>? _logger;
        private readonly SessionOptions _options;
        private readonly ConcurrentDictionary<string, SessionContext> _sessionMap = new();

        private IBrowser? _browser;

        public BrowserContextManager(
            string browserName,
            SessionOptions options,
            ILoggerFactory? loggerFactory = null)
        {
            _browserName = browserName;
            _logger = loggerFactory?.CreateLogger<BrowserContextManager>();

            _options = options;
        }

        public IBrowser? Browser => _browser;

        public async Task InitializeAsync()
        {
            var playwright = await Playwright
                .CreateAsync()
                .ConfigureAwait(false);
            _browser = await (_browserName.ToLowerInvariant() switch
                    {
                        "chromium" => playwright.Chromium,
                        "chrome" => playwright.Chromium,
                        "firefox" => playwright.Firefox,
                        _ => throw new NotImplementedException(),
                    })
                .LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = _options.Headless,
                })
                .ConfigureAwait(false);
        }

        public async Task SetOrCreateSessionAsync(
            IScenarioContext scenarioContext,
            UserCredential credential,
            Func<IScenarioContext, Task> authenticationFlow,
            Func<IScenarioContext, Task> testStartingPointFlow)
        {
            if (scenarioContext == null)
            {
                throw new ArgumentNullException(nameof(scenarioContext));
            }

            if (credential == null)
            {
                credential = scenarioContext.GetCurrentUser();
            }
            else
            {
                scenarioContext.SetCurrentUser(credential);
            }

            var existingSession = _sessionMap
                .GetOrAdd(
                    credential.Username,
                    new SessionContext(_browser, credential, _options));

            await existingSession
                .GetBrowserPageAsync(scenarioContext, authenticationFlow, testStartingPointFlow)
                .ConfigureAwait(false);
        }

        public ValueTask DisposeAsync()
        {
            foreach (var session in _sessionMap)
            {
                session.Value?.Dispose();
            }

            _browser?.DisposeAsync();

            return ValueTask.CompletedTask;
        }
    }
}
