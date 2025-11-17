using CommandLine;

using System.Text.Json;

using Io.Cucumber.Messages.Types;
using Cucumber.Messages;

namespace WhateverDotNet.Reporting.JsonFormatter;

public class Program
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Path to directory containing cucumber messages")]
        public string? InputPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "Path to directory containing cucumber json reports")]
        public string? OutputPath { get; set; }
    }

    private static Dictionary<string, InternalTestCase> testCaseById = new();
    private static Dictionary<string, Cucumber.Contracts.Feature> jsonFeaturesByURI = new();
    private static List<Cucumber.Contracts.Feature> jsonFeatures = new();
    private static MessageLookup lookup = new();
    private static bool verbose = false;

    public static async Task Main(string[] args)
    {
        (await Parser
                .Default
                .ParseArguments<Options>(args)
                .WithParsedAsync(RunWithOptions))
            .WithNotParsed(HandleParseError);
    }

    private static async Task RunWithOptions(Options opts)
    {
        if (!Directory.Exists(opts.InputPath))
        {
            throw new DirectoryNotFoundException();
        }

        string[] messagesFiles = Directory.GetFiles(opts.InputPath!, "*.ndjson", SearchOption.AllDirectories);

        lookup.Initialize(verbose);

        foreach (string messagesFile in messagesFiles)
        {
            using var stream = File.OpenRead(messagesFile);
            using var messageReader = new NdjsonMessageReader(stream, NdjsonSerializer.Deserialize);

            foreach (Envelope envelope in messageReader)
            {
                if (envelope == null)
                {
                    continue;
                }

                lookup.ProcessMessage(envelope);

                if (envelope.TestCaseStarted != null)
                {
                    var testCase = TestCaseProcessor.ProcessTestCaseStarted(envelope.TestCaseStarted, lookup);
                    testCaseById[testCase.TestCase.Id] = testCase;
                }

                if (envelope.TestStepFinished != null)
                {
                    var testStep = TestStepProcessor.ProcessTestStepFinished(envelope.TestStepFinished, lookup);
                    if (!testCaseById.TryGetValue(testStep.TestCaseID, out var testCase))
                    {
                        var keys = string.Join(", ", testCaseById.Keys);
                        throw new System.Exception("No testCase for " + testStep.TestCaseID + keys);
                    }
                    testCase.AppendStep(testStep);
                }

                if (envelope.TestCaseFinished != null)
                {
                    var testCaseStarted = lookup.LookupTestCaseStarted(envelope.TestCaseFinished.TestCaseStartedId);
                    if (testCaseStarted != null && testCaseById.TryGetValue(testCaseStarted.TestCaseId, out var testCase))
                    {
                        var jsonFeature = FindOrCreateJsonFeature(testCase.Pickle);
                        foreach (var jsonElement in TestCaseProcessor.TestCaseToJSON(testCase))
                        {
                            jsonFeature.Elements.Add(jsonElement);
                        }
                    }
                }
            }
        }

        var outputJson = JsonSerializer.Serialize(jsonFeatures, new JsonSerializerOptions { WriteIndented = true });
        if (!string.IsNullOrEmpty(opts.OutputPath))
        {
            await File.WriteAllTextAsync(opts.OutputPath, outputJson);
        }
        else
        {
            Console.WriteLine(outputJson);
        }
    }

    private static Cucumber.Contracts.Feature FindOrCreateJsonFeature(Pickle pickle)
    {
        if (!jsonFeaturesByURI.TryGetValue(pickle.Uri, out var jFeature))
        {
            var gherkinDocumentFeature = lookup.LookupGherkinDocument(pickle.Uri)?.Feature;
            if (gherkinDocumentFeature == null)
            {
                throw new System.Exception($"No feature found for URI: {pickle.Uri}");
            }

            jFeature = new Cucumber.Contracts.Feature
            {
                Description = gherkinDocumentFeature.Description,
                Elements = new List<Cucumber.Contracts.Scenario>(),
                ID = MakeId(gherkinDocumentFeature.Name),
                Keyword = gherkinDocumentFeature.Keyword,
                Line = (uint)gherkinDocumentFeature.Location.Line,
                Name = gherkinDocumentFeature.Name,
                URI = pickle.Uri,
                Tags = new List<Cucumber.Contracts.Tag>(gherkinDocumentFeature.Tags.Count)
            };

            for (int tagIndex = 0; tagIndex < gherkinDocumentFeature.Tags.Count; tagIndex++)
            {
                var tag = gherkinDocumentFeature.Tags[tagIndex];
                jFeature.Tags.Add(new Cucumber.Contracts.Tag
                {
                    Line = (uint)tag.Location.Line,
                    Name = tag.Name
                });
            }

            jsonFeaturesByURI[pickle.Uri] = jFeature;
            jsonFeatures.Add(jFeature);
        }
        return jFeature;
    }

    private static string MakeId(string s)
    {
        return s.ToLowerInvariant().Replace(" ", "-");
    }

    private static void Comment(string message)
    {
        if (verbose)
        {
            Console.WriteLine($"// Formatter: {message}");
        }
    }

    private static void HandleParseError(IEnumerable<Error> errors)
    {
    }
}