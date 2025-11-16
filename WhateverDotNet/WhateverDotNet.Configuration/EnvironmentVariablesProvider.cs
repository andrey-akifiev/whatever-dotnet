using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace WhateverDotNet.Configuration
{
    public class EnvironmentVariablesProvider<TVariables>
        where TVariables : class, new()
    {
        private static readonly Dictionary<string, Action<TVariables, object?>> _setters = BuildSetters();
        private static readonly ConcurrentDictionary<TestEnvironment, TVariables> _cache = new();

        private readonly EnvironmentOptions _options;

        public EnvironmentVariablesProvider(EnvironmentOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public TVariables Get(TestEnvironment environment)
        {
            return _cache.GetOrAdd(environment, CreateFromEnvironment);
        }

        // Analyze T once and prepare property setters
        private static Dictionary<string, Action<TVariables, object?>> BuildSetters()
        {
            var dict = new Dictionary<string, Action<TVariables, object?>>();
            var props = typeof(TVariables)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var prop in props)
            {
                var target = Expression.Parameter(typeof(TVariables), "target");
                var value = Expression.Parameter(typeof(object), "value");
                var convert = Expression.Convert(value, prop.PropertyType);
                var assign = Expression.Assign(Expression.Property(target, prop), convert);
                var lambda = Expression.Lambda<Action<TVariables, object?>>(assign, target, value);
                dict[prop.Name] = lambda.Compile();
            }

            return dict;
        }

        // Create a new instance and set values from environment variables
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
    }
}