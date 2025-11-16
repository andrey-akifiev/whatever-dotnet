using Microsoft.Playwright;

using WhateverDotNet.Abstractions;
using WhateverDotNet.Abstractions.Exceptions;
using WhateverDotNet.Components.Panes;

namespace WhateverDotNet.Components.Pages
{
    public abstract class BasePage : BasePane<PageOptions>, IApplicationPane
    {
        protected BasePage(IScenarioContext scenarioContext, PageOptions options)
            : base(scenarioContext, options)
        {
        }

        public override PageOptions GetOptions()
        {
            return (PageOptions)_options;
        }

        public virtual async Task NavigateAsync()
        {
            string[]? navigationLinks = GetOptions().NavigationLinks;

            if (navigationLinks == null || navigationLinks.Length == 0)
            {
                throw new OptionNotDefinedException(nameof(_options.NavigationLinks));
            }

            await GetPage()
                .WaitForTimeoutAsync(100)
                .ConfigureAwait(false);

            foreach (string link in navigationLinks)
            {
                await GetLocator(link)
                    .ClickAsync(new LocatorClickOptions { Force = true })
                    .ConfigureAwait(false);
            }

            _scenarioContext.SetCurrentPage(this);

            await WaitUntilLoadedAsync().ConfigureAwait(false);
        }

        public virtual async Task VisitAsync(int timeoutInMiliseconds = 30000)
        {
            var urlOption = GetOptions().Url;

            if (string.IsNullOrWhiteSpace(urlOption))
            {
                throw new OptionNotDefinedException(nameof(_options.Url));
            }

            await GetPage()
                .GotoAsync(urlOption, new PageGotoOptions { Timeout = timeoutInMiliseconds })
                .ConfigureAwait(false);

            _scenarioContext.SetCurrentPage(this);
        }

        PaneOptions IApplicationPane<PaneOptions>.GetOptions()
        {
            return GetOptions();
        }
    }
}
