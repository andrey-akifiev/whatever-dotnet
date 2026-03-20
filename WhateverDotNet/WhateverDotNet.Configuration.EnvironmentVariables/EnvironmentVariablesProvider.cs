using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace WhateverDotNet.Configuration;

/// <summary>
/// Creates strongly typed configuration objects by reading values from environment variables.
/// </summary>
/// <typeparam name="TVariables">
/// A reference type with a public parameterless constructor.
/// Writable public properties on <typeparamref name="TVariables"/> are used as mapping targets.
/// </typeparam>
public class EnvironmentVariablesProvider<TVariables>(EnvironmentOptions options)
    where TVariables : class, new()
{
    private static readonly Dictionary<string, Action<TVariables, object?>> _setters = BuildSetters();
    private static readonly ConcurrentDictionary<TestEnvironment, TVariables> _cache = new();

    private readonly EnvironmentOptions _options = options
        ?? throw new ArgumentNullException(nameof(options));

    /// <summary>
    /// Gets (and caches) a configuration instance for the specified environment.
    /// </summary>
    /// <param name="environment">Test execution environment used to pick env-var suffixes.</param>
    /// <returns>A populated instance of <typeparamref name="TVariables"/>.</returns>
    public TVariables Get(TestEnvironment environment)
    {
        return _cache.GetOrAdd(environment, CreateFromEnvironment);
    }

    // Create a new instance and set values from environment variables
    /// <summary>
    /// Creates a new configuration instance by reading environment variables for the given environment.
    /// </summary>
    /// <param name="environment">Test execution environment used to pick env-var suffixes.</param>
    /// <returns>A populated instance of <typeparamref name="TVariables"/>.</returns>
    public TVariables CreateFromEnvironment(TestEnvironment environment)
    {
        string normalizedEnvPostfix = JsonNamingPolicy.SnakeCaseUpper.ConvertName(environment.ToString());

        IDictionary rawEnvVariables = Environment.GetEnvironmentVariables();

        var instance = new TVariables();
        foreach (var kvp in _setters)
        {
            string propName = kvp.Key;
            Action<TVariables, object?> setter = kvp.Value;

            string normalizedEnvVarName = JsonNamingPolicy.SnakeCaseUpper.ConvertName(propName);
            string primaryEnvVar = $"{_options.ProjectPrefix}{normalizedEnvVarName}_{normalizedEnvPostfix}";
            string fallbackEnvVar = $"{_options.ProjectPrefix}{normalizedEnvVarName}";

            object? envValue = rawEnvVariables.Contains(primaryEnvVar)
                ? rawEnvVariables[primaryEnvVar]
                : rawEnvVariables.Contains(fallbackEnvVar)
                    ? rawEnvVariables[fallbackEnvVar]
                    : null;

            if (envValue != null)
            {
                setter(instance, envValue);
            }
        }

        return instance;
    }
    
    // Analyze T once and prepare property setters
    private static Dictionary<string, Action<TVariables, object?>> BuildSetters()
    {
        Dictionary<string, Action<TVariables, object?>> dict = new ();
        IEnumerable<PropertyInfo> props = typeof(TVariables)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite);

        foreach (PropertyInfo prop in props)
        {
            ParameterExpression target = Expression.Parameter(typeof(TVariables), "target");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");
            UnaryExpression convert = Expression.Convert(value, prop.PropertyType);
            BinaryExpression assign = Expression.Assign(Expression.Property(target, prop), convert);
            Expression<Action<TVariables, object?>> lambda = Expression.Lambda<Action<TVariables, object?>>(assign, target, value);
            dict[prop.Name] = lambda.Compile();
        }

        return dict;
    }
}