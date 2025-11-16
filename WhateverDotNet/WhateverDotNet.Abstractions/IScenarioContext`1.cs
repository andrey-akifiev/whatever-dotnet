namespace WhateverDotNet.Abstractions;

public interface IScenarioContext<TEnvironmentVariables> : IScenarioContext
{
    public TEnvironmentVariables EnvironmentVariables { get; }
}