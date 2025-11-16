using WhateverDotNet.Abstractions;

namespace WhateverDotNet.Engine.Contexts;

public class ScenarioContext<TEnvironmentVariables> : ScenarioContext, IScenarioContext, IScenarioContext<TEnvironmentVariables>
    where TEnvironmentVariables : class, new()
{
    public TEnvironmentVariables EnvironmentVariables => GetTestRunContext().EnvironmentVariables;

    public override TestRunContext<TEnvironmentVariables> GetTestRunContext()
    {
        return (_testRunContext as TestRunContext<TEnvironmentVariables>)!;
    }
}