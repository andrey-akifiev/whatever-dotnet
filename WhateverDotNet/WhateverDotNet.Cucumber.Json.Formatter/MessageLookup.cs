using Io.Cucumber.Messages.Types;

namespace WhateverDotNet.Cucumber.Json.Formatter;

public class MessageLookup
{
    private Dictionary<string, GherkinDocument> gherkinDocumentByURI = new();
    private Dictionary<string, Pickle> pickleByID = new();
    private Dictionary<string, PickleStep> pickleStepByID = new();
    private Dictionary<string, TestCase> testCaseByID = new();
    private Dictionary<string, TestStep> testStepByID = new();
    private Dictionary<string, TestCaseStarted> testCaseStartedByID = new();
    private Dictionary<string, Step> stepByID = new();
    private Dictionary<string, Scenario> scenarioByID = new();
    private Dictionary<string, Examples> exampleByRowID = new();
    private Dictionary<string, TableRow> exampleRowByID = new();
    private Dictionary<string, StepDefinition> stepDefinitionByID = new();
    private Dictionary<string, Background> backgroundByStepID = new();
    private Dictionary<string, Tag> tagByID = new();
    private Dictionary<string, Hook> hookByID = new();
    private Dictionary<string, List<Attachment>> attachmentsByTestStepID = new();
    private bool verbose;

    public void Initialize(bool verbose)
    {
        gherkinDocumentByURI = new();
        pickleByID = new();
        pickleStepByID = new();
        testCaseByID = new();
        testStepByID = new();
        testCaseStartedByID = new();
        stepByID = new();
        scenarioByID = new();
        exampleByRowID = new();
        exampleRowByID = new();
        stepDefinitionByID = new();
        backgroundByStepID = new();
        tagByID = new();
        hookByID = new();
        attachmentsByTestStepID = new();
        this.verbose = verbose;
    }

    public void ProcessMessage(Envelope envelope)
    {
        if (envelope.GherkinDocument != null)
        {
            gherkinDocumentByURI[envelope.GherkinDocument.Uri] = envelope.GherkinDocument;
            Comment($"Stored GherkinDocument: {envelope.GherkinDocument.Uri}");
            foreach (var key in gherkinDocumentByURI.Keys)
            {
                Comment($" - {key} ");
            }
            if (envelope.GherkinDocument.Feature != null)
            {
                ProcessTags(envelope.GherkinDocument.Feature.Tags);
                foreach (var child in envelope.GherkinDocument.Feature.Children)
                {
                    ProcessRule(child.Rule);
                    ProcessBackground(child.Background);
                    ProcessScenario(child.Scenario);
                }
            }
        }

        if (envelope.Pickle != null)
        {
            pickleByID[envelope.Pickle.Id] = envelope.Pickle;
            foreach (var step in envelope.Pickle.Steps)
            {
                pickleStepByID[step.Id] = step;
            }
        }

        if (envelope.TestCase != null)
        {
            testCaseByID[envelope.TestCase.Id] = envelope.TestCase;
            foreach (var step in envelope.TestCase.TestSteps)
            {
                testStepByID[step.Id] = step;
            }
        }

        if (envelope.TestCaseStarted != null)
        {
            testCaseStartedByID[envelope.TestCaseStarted.Id] = envelope.TestCaseStarted;
        }

        if (envelope.Attachment != null)
        {
            if (!attachmentsByTestStepID.TryGetValue(envelope.Attachment.TestStepId, out var attachments))
            {
                attachments = new List<Attachment>();
                attachmentsByTestStepID[envelope.Attachment.TestStepId] = attachments;
            }
            attachments.Add(envelope.Attachment);
        }

        if (envelope.StepDefinition != null)
        {
            stepDefinitionByID[envelope.StepDefinition.Id] = envelope.StepDefinition;
        }

        if (envelope.Hook != null)
        {
            hookByID[envelope.Hook.Id] = envelope.Hook;
        }
    }

    private void ProcessTags(List<Tag> tags)
    {
        if (tags == null) return;
        foreach (var tag in tags)
        {
            tagByID[tag.Id] = tag;
        }
    }

    private void ProcessRule(Rule rule)
    {
        if (rule != null)
        {
            foreach (var ruleChild in rule.Children)
            {
                ProcessBackground(ruleChild.Background);
                ProcessScenario(ruleChild.Scenario);
            }
        }
    }

    private void ProcessBackground(Background background)
    {
        if (background != null)
        {
            foreach (var step in background.Steps)
            {
                backgroundByStepID[step.Id] = background;
                stepByID[step.Id] = step;
            }
        }
    }

    private void ProcessScenario(Scenario scenario)
    {
        if (scenario != null)
        {
            scenarioByID[scenario.Id] = scenario;
            ProcessTags(scenario.Tags);
            foreach (var step in scenario.Steps)
            {
                stepByID[step.Id] = step;
            }
            foreach (var example in scenario.Examples)
            {
                ProcessTags(example.Tags);
                foreach (var row in example.TableBody)
                {
                    exampleByRowID[row.Id] = example;
                    exampleRowByID[row.Id] = row;
                }
            }
        }
    }

    public GherkinDocument LookupGherkinDocument(string uri)
    {
        if (gherkinDocumentByURI.TryGetValue(uri, out var item))
        {
            InformFoundKey(uri, nameof(gherkinDocumentByURI));
        }
        else
        {
            InformMissingKey(uri, nameof(gherkinDocumentByURI));
        }
        return item;
    }

    public Scenario LookupScenario(string id)
    {
        if (scenarioByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(scenarioByID));
        }
        else
        {
            InformMissingKey(id, nameof(scenarioByID));
        }
        return item;
    }

    public Pickle LookupPickle(string id)
    {
        if (pickleByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(pickleByID));
        }
        else
        {
            InformMissingKey(id, nameof(pickleByID));
        }
        return item;
    }

    public Step LookupStep(string id)
    {
        if (stepByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(stepByID));
        }
        else
        {
            InformMissingKey(id, nameof(stepByID));
        }
        return item;
    }

    public Examples LookupExample(string id)
    {
        if (exampleByRowID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(exampleByRowID));
        }
        else
        {
            InformMissingKey(id, nameof(exampleByRowID));
        }
        return item;
    }

    public TableRow LookupExampleRow(string id)
    {
        if (exampleRowByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(exampleRowByID));
        }
        else
        {
            InformMissingKey(id, nameof(exampleRowByID));
        }
        return item;
    }

    public Background LookupBackgroundByStepID(string id)
    {
        if (backgroundByStepID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(backgroundByStepID));
        }
        else
        {
            InformMissingKey(id, nameof(backgroundByStepID));
        }
        return item;
    }

    public Tag LookupTag(string id)
    {
        if (tagByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(tagByID));
        }
        else
        {
            InformMissingKey(id, nameof(tagByID));
        }
        return item;
    }

    public TestCaseStarted LookupTestCaseStarted(string id)
    {
        if (testCaseStartedByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(testCaseStartedByID));
        }
        else
        {
            InformMissingKey(id, nameof(testCaseStartedByID));
        }
        return item;
    }

    public TestCase LookupTestCase(string id)
    {
        if (testCaseByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(testCaseByID));
        }
        else
        {
            InformMissingKey(id, nameof(testCaseByID));
        }
        return item;
    }

    public TestStep LookupTestStep(string id)
    {
        if (testStepByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(testStepByID));
        }
        else
        {
            InformMissingKey(id, nameof(testStepByID));
        }
        return item;
    }

    public PickleStep LookupPickleStep(string id)
    {
        if (pickleStepByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(pickleStepByID));
        }
        else
        {
            InformMissingKey(id, nameof(pickleStepByID));
        }
        return item;
    }

    public List<StepDefinition> LookupStepDefinitions(List<string> ids)
    {
        var stepDefinitions = new List<StepDefinition>(ids.Count);
        foreach (var id in ids)
        {
            stepDefinitions.Add(LookupStepDefinition(id));
        }
        return stepDefinitions;
    }

    public StepDefinition LookupStepDefinition(string id)
    {
        if (stepDefinitionByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(stepDefinitionByID));
        }
        else
        {
            InformMissingKey(id, nameof(stepDefinitionByID));
        }
        return item;
    }

    public Hook LookupHook(string id)
    {
        if (hookByID.TryGetValue(id, out var item))
        {
            InformFoundKey(id, nameof(hookByID));
        }
        else
        {
            InformMissingKey(id, nameof(hookByID));
        }
        return item;
    }

    public List<Attachment> LookupAttachments(string testStepId)
    {
        if (attachmentsByTestStepID.TryGetValue(testStepId, out var item))
        {
            InformFoundKey(testStepId, nameof(attachmentsByTestStepID));
        }
        else
        {
            InformMissingKey(testStepId, nameof(attachmentsByTestStepID));
        }
        return item;
    }

    private void InformFoundKey(string key, string mapName)
    {
        Comment($"Found item '{key}' in {mapName}");
    }

    private void InformMissingKey(string key, string mapName)
    {
        Comment($"Unable to find '{key}' in {mapName}");
    }

    private void Comment(string message)
    {
        if (verbose)
        {
            Console.WriteLine($"// LookUp: {message}");
        }
    }
}