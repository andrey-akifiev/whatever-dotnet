using Microsoft.Playwright;

namespace WhateverDotNet.Engine.Contexts
{
    public class TestRunContext
    {
        private readonly string _id = Guid.NewGuid().ToString();
        private readonly BrowserContextManager _browserContextManager;

        public TestRunContext(BrowserContextManager browserContextManager)
        {
            _browserContextManager = browserContextManager ?? throw new ArgumentNullException(nameof(browserContextManager));
        }

        public IBrowser? Browser => _browserContextManager?.Browser;

        internal BrowserContextManager BrowserContextManager => _browserContextManager;
    }
}
