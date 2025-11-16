using WhateverDotNet.Configuration;

namespace WhateverDotNet.Engine.Contexts
{
    public class TestRunContext<TEnvironmentVariables> : TestRunContext
        where TEnvironmentVariables : class, new()
    {
        private readonly TestRunOptions _options;

        private EnvironmentVariablesProvider<TEnvironmentVariables>? _environmentVariablesProvider;

        public TestRunContext(TestRunOptions options, BrowserContextManager browserContextManager)
            : base(browserContextManager)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _environmentVariablesProvider = new EnvironmentVariablesProvider<TEnvironmentVariables>(
                new EnvironmentOptions { ProjectPrefix = _options.EnvironmentVariablesProjectPrefix });
        }

        public TEnvironmentVariables EnvironmentVariables => _environmentVariablesProvider!.Get(_options.TestEnvironment);
    }
}