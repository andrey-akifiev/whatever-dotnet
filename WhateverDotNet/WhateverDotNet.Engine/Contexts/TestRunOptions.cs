using WhateverDotNet.Configuration;

namespace WhateverDotNet.Engine.Contexts
{
    public class TestRunOptions
    {
        public string? EnvironmentVariablesProjectPrefix { get; set; }

        public TestEnvironment TestEnvironment { get; set; }
    }
}