using System.Text;
using Io.Cucumber.Messages.Types;

namespace WhateverDotNet.Reporting.JsonFormatter;

internal static class TestStepProcessor
{
    public static InternalTestStep ProcessTestStepFinished(TestStepFinished testStepFinished, MessageLookup lookup)
    {
        var testCaseStarted = lookup.LookupTestCaseStarted(testStepFinished.TestCaseStartedId);
        if (testCaseStarted == null)
        {
            throw new System.Exception("No testCaseStarted for " + testStepFinished.TestCaseStartedId);
        }

        var testCase = lookup.LookupTestCase(testCaseStarted.TestCaseId);
        if (testCase == null)
        {
            throw new System.Exception("No testCase for " + testCaseStarted.TestCaseId);
        }

        var testStep = lookup.LookupTestStep(testStepFinished.TestStepId);
        if (testStep == null)
        {
            throw new System.Exception("No testStep for " + testStepFinished.TestStepId);
        }

        if (!string.IsNullOrEmpty(testStep.HookId))
        {
            var hook = lookup.LookupHook(testStep.HookId);
            if (hook == null)
            {
                throw new System.Exception("No hook for " + testStep.HookId);
            }

            return new InternalTestStep
            {
                TestCaseID = testCase.Id,
                Hook = hook,
                Result = testStepFinished.TestStepResult,
                Attachments = lookup.LookupAttachments(testStepFinished.TestStepId)
            };
        }

        var pickle = lookup.LookupPickle(testCase.PickleId);
        if (pickle == null)
        {
            throw new System.Exception("No pickle for " + testCase.PickleId);
        }

        var pickleStep = lookup.LookupPickleStep(testStep.PickleStepId);
        if (pickleStep == null)
        {
            throw new System.Exception("No pickleStep for " + testStep.PickleStepId);
        }

        TableRow? exampleRow = null;
        if (pickle.AstNodeIds != null && pickle.AstNodeIds.Count > 1)
        {
            exampleRow = lookup.LookupExampleRow(pickle.AstNodeIds[1]);
        }

        Background? background = null;
        var scenarioStep = lookup.LookupStep(pickleStep.AstNodeIds[0]);
        if (scenarioStep != null)
        {
            background = lookup.LookupBackgroundByStepID(scenarioStep.Id);
        }

        return new InternalTestStep
        {
            TestCaseID = testCase.Id,
            Step = lookup.LookupStep(pickleStep.AstNodeIds[0]),
            Pickle = pickle,
            PickleStep = pickleStep,
            ExampleRow = exampleRow,
            Result = testStepFinished.TestStepResult,
            StepDefinitions = lookup.LookupStepDefinitions(testStep.StepDefinitionIds),
            Background = background,
            Attachments = lookup.LookupAttachments(testStepFinished.TestStepId)
        };
    }

    public static Cucumber.Contracts.Step TestStepToJSON(InternalTestStep step)
    {
        var status = step.Result.Status.ToString().ToLowerInvariant();
        ulong duration = 0;
        if (step.Result.Duration != null)
        {
            duration = (ulong)step.Result.Duration.Nanos;
        }

        if (step.Hook != null)
        {
            return new Cucumber.Contracts.Step
            {
                Match = new Cucumber.Contracts.StepMatch
                {
                    Location = MakeSourceReferenceLocation(step.Hook.SourceReference)
                },
                Result = new Cucumber.Contracts.StepResult
                {
                    Status = status,
                    ErrorMessage = step.Result.Message,
                    Duration = duration
                },
                Embeddings = MakeEmbeddings(step.Attachments)
            };
        }

        var location = MakeLocation(step.Pickle.Uri, step.Step.Location.Line);
        if (step.ExampleRow != null)
        {
            location = MakeLocation(step.Pickle.Uri, step.ExampleRow.Location.Line);
        }

        if (step.StepDefinitions != null && step.StepDefinitions.Count == 1)
        {
            location = MakeSourceReferenceLocation(step.StepDefinitions[0].SourceReference);
        }

        var jsonStep = new Cucumber.Contracts.Step
        {
            Keyword = step.Step.Keyword,
            Name = step.PickleStep.Text,
            Line = (uint)step.Step.Location.Line,
            Match = new Cucumber.Contracts.StepMatch
            {
                Location = location
            },
            Result = new Cucumber.Contracts.StepResult
            {
                Status = status,
                ErrorMessage = step.Result.Message,
                Duration = duration
            },
            Embeddings = MakeEmbeddings(step.Attachments),
            Output = MakeOutput(step.Attachments)
        };

        var docString = step.Step.DocString;
        if (docString != null)
        {
            jsonStep.DocString = new Cucumber.Contracts.DocString
            {
                Line = (uint)docString.Location.Line,
                ContentType = docString.MediaType,
                Value = docString.Content
            };
        }

        var datatable = step.Step.DataTable;
        if (datatable != null)
        {
            jsonStep.Rows = new List<Cucumber.Contracts.DatatableRow>();
            foreach (var row in datatable.Rows)
            {
                var cells = new List<string>();
                foreach (var cell in row.Cells)
                {
                    cells.Add(cell.Value);
                }

                jsonStep.Rows.Add(new Cucumber.Contracts.DatatableRow
                {
                    Cells = cells
                });
            }
        }

        return jsonStep;
    }

    public static List<Cucumber.Contracts.Embedding> MakeEmbeddings(List<Attachment>? attachments)
    {
        var embeddableAttachments = FilterAttachments(attachments, IsEmbeddable);
        var jsonEmbeddings = new List<Cucumber.Contracts.Embedding>();

        foreach (var attachment in embeddableAttachments)
        {
            string data;
            if (attachment.ContentEncoding == AttachmentContentEncoding.BASE64)
            {
                data = attachment.Body;
            }
            else
            {
                data = Convert.ToBase64String(Encoding.UTF8.GetBytes(attachment.Body));
            }

            jsonEmbeddings.Add(new Cucumber.Contracts.Embedding
            {
                Data = data,
                MimeType = attachment.MediaType
            });
        }

        return jsonEmbeddings;
    }

    public static List<string> MakeOutput(List<Attachment>? attachments)
    {
        var outputAttachments = FilterAttachments(attachments, IsOutput);
        var output = new List<string>();

        foreach (var attachment in outputAttachments)
        {
            output.Add(attachment.Body);
        }

        return output;
    }

    public static List<Attachment> FilterAttachments(List<Attachment>? attachments, Func<Attachment, bool> filter)
    {
        var matches = new List<Attachment>();
        if (attachments == null)
        {
            return matches;
        }
        foreach (var attachment in attachments)
        {
            if (filter(attachment))
            {
                matches.Add(attachment);
            }
        }
        return matches;
    }

    public static bool IsEmbeddable(Attachment attachment)
    {
        return !IsOutput(attachment);
    }

    public static bool IsOutput(Attachment attachment)
    {
        return attachment.MediaType == "text/x.cucumber.log+plain";
    }

    public static string MakeLocation(string file, long line)
    {
        return $"{file}:{line}";
    }

    public static string MakeJavaMethodLocation(JavaMethod javaMethod)
    {
        var typeList = string.Join(",", javaMethod.MethodParameterTypes);
        return $"{javaMethod.ClassName}.{javaMethod.MethodName}({typeList})";
    }

    public static string MakeJavaStackTraceElementLocation(JavaStackTraceElement javaStackTraceElement, Location? location)
    {
        if (location != null)
        {
            return MakeLocation(javaStackTraceElement.FileName, location.Line);
        }
        return javaStackTraceElement.FileName;
    }

    public static string MakeSourceReferenceLocation(SourceReference sourceReference)
    {
        var javaMethod = sourceReference.JavaMethod;
        if (javaMethod != null)
        {
            return MakeJavaMethodLocation(javaMethod);
        }

        var location = sourceReference.Location;
        var javaStackTraceElement = sourceReference.JavaStackTraceElement;
        if (javaStackTraceElement != null)
        {
            return MakeJavaStackTraceElementLocation(javaStackTraceElement, location);
        }

        return MakeLocation(sourceReference.Uri, location.Line);
    }
}