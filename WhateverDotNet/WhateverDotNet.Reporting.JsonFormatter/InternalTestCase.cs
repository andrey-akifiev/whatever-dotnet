using Io.Cucumber.Messages.Types;

namespace WhateverDotNet.Reporting.JsonFormatter;
    
public class InternalTestCase
{
    public string FeatureName { get; set; }
    public Scenario Scenario { get; set; }
    public Pickle Pickle { get; set; }
    public TestCase TestCase { get; set; }
    public List<InternalTestStep> Steps { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();

    public void AppendStep(InternalTestStep step)
    {
        Steps.Add(step);
    }

    public SortedSteps SortedSteps()
    {
        var sorted = new SortedSteps();
        List<InternalTestStep> current = sorted.BeforeHook;

        foreach (var step in Steps)
        {
            if (ReferenceEquals(current, sorted.BeforeHook) && step.Hook == null)
            {
                current = sorted.Background;
            }
            if (ReferenceEquals(current, sorted.Background) && step.Background == null)
            {
                current = sorted.Steps;
            }
            if (ReferenceEquals(current, sorted.Steps) && step.Hook != null)
            {
                current = sorted.AfterHook;
            }
            current.Add(step);
        }

        return sorted;
    }
}

public class SortedSteps
{
    public List<InternalTestStep> BeforeHook { get; set; } = new();
    public List<InternalTestStep> Background { get; set; } = new();
    public List<InternalTestStep> Steps { get; set; } = new();
    public List<InternalTestStep> AfterHook { get; set; } = new();
}

public static class TestCaseProcessor
{
    public static InternalTestCase ProcessTestCaseStarted(TestCaseStarted testCaseStarted, MessageLookup lookup)
    {
        var testCase = lookup.LookupTestCase(testCaseStarted.TestCaseId);
        if (testCase == null)
        {
            throw new System.Exception("No testCase for " + testCaseStarted.TestCaseId);
        }

        var pickle = lookup.LookupPickle(testCase.PickleId);
        if (pickle == null || pickle.AstNodeIds == null || pickle.AstNodeIds.Count == 0)
        {
            throw new System.Exception("No pickle for " + testCase.PickleId);
        }

        var tags = new List<Tag>();
        for (int index = 0; index < pickle.Tags.Count; index++)
        {
            var tag = pickle.Tags[index];
            var sourceTag = lookup.LookupTag(tag.AstNodeId);
            if (sourceTag == null)
            {
                throw new System.Exception("No sourceTag for " + tag.AstNodeId);
            }
            tags.Add(sourceTag);
        }

        var scenario = lookup.LookupScenario(pickle.AstNodeIds[0]);
        if (scenario == null)
        {
            throw new System.Exception($"No scenario for {string.Join(", ", pickle.AstNodeIds)}");
        }

        var feature = lookup.LookupGherkinDocument(pickle.Uri);
        if (feature == null)
        {
            throw new System.Exception("No feature for " + pickle.Uri);
        }
        var featureName = feature.Feature.Name;

        return new InternalTestCase
        {
            FeatureName = featureName,
            Scenario = scenario,
            Pickle = pickle,
            TestCase = testCase,
            Steps = new List<InternalTestStep>(),
            Tags = tags
        };
    }

    public static List<Cucumber.Contracts.Scenario> TestCaseToJSON(InternalTestCase testCase)
    {
        var elements = new List<Cucumber.Contracts.Scenario>();
        var sortedSteps = testCase.SortedSteps();

        if (sortedSteps.Background.Count > 0)
        {
            elements.Add(BackgroundStepsToJSON(sortedSteps.Background));
        }
        elements.Add(ScenarioStepsToJSON(testCase, sortedSteps.Steps));

        if (sortedSteps.BeforeHook.Count > 0)
        {
            elements[0].Before = MakeJSONSteps(sortedSteps.BeforeHook);
        }

        if (sortedSteps.AfterHook.Count > 0)
        {
            elements[elements.Count - 1].After = MakeJSONSteps(sortedSteps.AfterHook);
        }

        return elements;
    }

    public static Cucumber.Contracts.Scenario BackgroundStepsToJSON(List<InternalTestStep> steps)
    {
        var background = steps[0].Background;

        return new Cucumber.Contracts.Scenario
        {
            Keyword = background.Keyword,
            Name = background.Name,
            Description = background.Description,
            Line = (uint)background.Location.Line,
            Type = "background",
            Steps = MakeJSONSteps(steps)
        };
    }

    public static Cucumber.Contracts.Scenario ScenarioStepsToJSON(InternalTestCase testCase, List<InternalTestStep> steps)
    {
        var line = testCase.Scenario.Location.Line;
        var id = $"{MakeID(testCase.FeatureName)};{MakeID(testCase.Scenario.Name)}";
        if (testCase.Pickle.AstNodeIds != null && testCase.Pickle.AstNodeIds.Count > 1)
        {
            string exampleName = "";
            int exampleIndex = 0;

            foreach (var example in testCase.Scenario.Examples)
            {
                for (int index = 0; index < example.TableBody.Count; index++)
                {
                    var row = example.TableBody[index];
                    if (row.Id == testCase.Pickle.AstNodeIds[1])
                    {
                        line = row.Location.Line;
                        exampleName = example.Name;
                        exampleIndex = index + 2;
                    }
                }
            }
            id = $"{MakeID(testCase.FeatureName)};{MakeID(testCase.Scenario.Name)};{MakeID(exampleName)};{exampleIndex}";
        }

        return new Cucumber.Contracts.Scenario
        {
            ID = id,
            Keyword = testCase.Scenario.Keyword,
            Type = "scenario",
            Name = testCase.Scenario.Name,
            Description = testCase.Scenario.Description,
            Line = (uint)line,
            Steps = MakeJSONSteps(steps),
            Tags = MakeJSONTags(testCase.Tags)
        };
    }

    public static List<Cucumber.Contracts.Step> MakeJSONSteps(List<InternalTestStep> steps)
    {
        var jsonSteps = new List<Cucumber.Contracts.Step>(steps.Count);
        for (int index = 0; index < steps.Count; index++)
        {
            jsonSteps.Add(TestStepProcessor.TestStepToJSON(steps[index]));
        }
        return jsonSteps;
    }

    public static List<Cucumber.Contracts.Tag> MakeJSONTags(List<Tag> tags)
    {
        var jsonTags = new List<Cucumber.Contracts.Tag>(tags.Count);
        for (int index = 0; index < tags.Count; index++)
        {
            var tag = tags[index];
            jsonTags.Add(new Cucumber.Contracts.Tag
            {
                Name = tag.Name,
                Line = (uint)tag.Location.Line
            });
        }
        return jsonTags;
    }

    public static string MakeID(string s)
    {
        return s.ToLowerInvariant().Replace(" ", "-");
    }
}