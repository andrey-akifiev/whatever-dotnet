using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using WhateverDotNet.Abstractions;


namespace WhateverDotNet.Engine.Contexts
{
    internal class SessionContext : IDisposable
    {
        private readonly SemaphoreSlim _lockObject = new (1, 1);

        private readonly IBrowser _browser;
        private readonly ILogger<SessionContext>? _logger;
        private readonly SessionOptions _options;

        private IBrowserContext _browserContext;
        private string _browserSession;

        private IPage? _vacantPage;

        public SessionContext(
            IBrowser browser,
            UserCredential credential,
            SessionOptions options,
            ILoggerFactory? loggerFactory = null)
        {
            _browser = browser ?? throw new ArgumentNullException(nameof(browser));
            _logger = loggerFactory?.CreateLogger<SessionContext>();
            _options = options;
        }

        public async Task<IBrowserContext> GetBrowserContextAsync(
            IScenarioContext scenarioContext,
            Func<IScenarioContext, Task> authenticationFlow,
            CancellationToken cancellationToken = default)
        {
            await _lockObject.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (IsBrowserContextObjectValid())
                {
                    return _browserContext;
                }

                if (!string.IsNullOrWhiteSpace(_browserSession))
                {
                    try
                    {
                        _browserContext = await _browser
                            .NewContextAsync(new BrowserNewContextOptions { StorageState = _browserSession })
                            .ConfigureAwait(false);

                        if (IsBrowserContextObjectValid())
                        {
                            return _browserContext;
                        }
                    }
                    catch (Exception)
                    {
                        // do nothing
                        // TODO: Log it -- aa
                    }
                }

                if (scenarioContext != null && authenticationFlow != null)
                {
                    _browserContext = await scenarioContext
                        .Browser!
                        .NewContextAsync()
                        .ConfigureAwait(false);
                    scenarioContext.SetTestData(Constants.CurrentBrowserContextVariable, _browserContext);
                    var browserPage = await _browserContext
                        .NewPageAsync()
                        .ConfigureAwait(false);
                    scenarioContext.SetTestData(Constants.CurrentBrowserPageVariable, browserPage);

                    await authenticationFlow(scenarioContext).ConfigureAwait(false);
                }
                else
                {
                    _browserContext = await _browser
                        .NewContextAsync()
                        .ConfigureAwait(false);
                }

                if (_browserContext == null)
                {
                    throw new Exception(); // TODO: Specify the error -- aa
                }

                _browserSession = await _browserContext
                    .StorageStateAsync()
                    .ConfigureAwait(false);

                _vacantPage = _browserContext.Pages.Count > 0
                    ? _browserContext.Pages.Last()
                    : null;

                return _browserContext;
            }
            finally
            {
                _lockObject.Release();
            }
        }

        public async Task<IPage> GetBrowserPageAsync(
            IScenarioContext scenarioContext,
            Func<IScenarioContext, Task> authenticationFlow,
            Func<IScenarioContext, Task> testStartingPointFlow,
            CancellationToken cancellationToken = default)
        {
            var browserContext = await GetBrowserContextAsync(
                scenarioContext,
                authenticationFlow,
                cancellationToken).ConfigureAwait(false);

            await _lockObject
                .WaitAsync(cancellationToken)
                .ConfigureAwait(false);

            IPage page;

            try
            {
                if (_vacantPage != null)
                {
                    page = _vacantPage;
                    _vacantPage = null;
                }
                else
                {
                    page = await browserContext.NewPageAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                _lockObject.Release();
            }

            await ConfigureBrowserPageAsync(page).ConfigureAwait(false);

            scenarioContext?.SetTestData(Constants.CurrentBrowserPageVariable, page);

            if (scenarioContext != null && testStartingPointFlow != null)
            {
                await testStartingPointFlow(scenarioContext).ConfigureAwait(false);
            }

            return page;
        }

        public void Dispose()
        {
            _lockObject?.Dispose();
        }

        private async Task ConfigureBrowserPageAsync(IPage page)
        {
            await page
                .SetViewportSizeAsync(_options.Width, _options.Height)
                .ConfigureAwait(false);
            page.SetDefaultNavigationTimeout(_options.DefaultNavigationTimeoutInMilliseconds);
            page.SetDefaultTimeout(_options.DefaultActionTimeoutInMilliseconds);
        }

        private bool IsBrowserContextObjectValid()
        {
            return _browserContext != null
                && _browserContext.Pages.Any();
        }
    }
}
